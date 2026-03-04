using ISpanShop.Models.DTOs;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Interfaces;
using ProductAttribute = ISpanShop.Models.EfModels.Attribute;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ISpanShop.Repositories
{
	/// <summary>
	/// 分類規格 Repository 實作
	/// 直接操作 EF Core DbContext，處理 Attributes / AttributeOptions / CategoryAttributes 三張表
	/// </summary>
	public class CategorySpecRepository : ICategorySpecRepository
	{
		private readonly ISpanShopDBContext _db; // 請替換為你實際的 DbContext 類別名稱

		/// <summary>
		/// 建構子 - 注入 DbContext
		/// </summary>
		public CategorySpecRepository(ISpanShopDBContext db)
		{
			_db = db;
		}

		/// <summary>
		/// 取得所有分類規格，並透過 Join 帶出對應的 AttributeOptions 選項
		/// </summary>
		public IEnumerable<CategorySpecDto> GetAll()
		{
			// 先撈出所有 Attributes 主表資料
			var attributes = _db.Attributes
				.OrderBy(a => a.SortOrder)
				.ToList();

			// 撈出所有選項，之後在記憶體中做 GroupBy 以減少 N+1 查詢
			var allOptions = _db.AttributeOptions.ToList();

			// 同時撈出 CategoryAttributes 關聯與 Categories 名稱
			var allBindings = _db.CategoryAttributes
				.Join(_db.Categories,
					  ca => ca.CategoryId,
					  c  => c.Id,
					  (ca, c) => new { ca.AttributeId, c.Name })
				.ToList();

			// 組合成 DTO
			return attributes.Select(a => new CategorySpecDto
			{
				Id = a.Id,
				Name = a.Name,
				InputType = a.InputType,
				IsRequired = a.IsRequired,
				SortOrder = a.SortOrder,
				// 從已載入的選項集合中篩選屬於此規格的選項
				Options = allOptions
					.Where(o => o.AttributeId == a.Id)
					.OrderBy(o => o.SortOrder)
					.Select(o => o.OptionName)
					.ToList(),
				BoundCategoryNames = allBindings
					.Where(b => b.AttributeId == a.Id)
					.Select(b => b.Name)
					.ToList()
			}).ToList();
		}

		/// <summary>
		/// 根據 ID 取得單一分類規格（含選項）
		/// </summary>
		public CategorySpecDto? GetById(int id)
		{
			var attr = _db.Attributes.FirstOrDefault(a => a.Id == id);
			if (attr == null) return null;

			var options = _db.AttributeOptions
				.Where(o => o.AttributeId == id)
				.OrderBy(o => o.SortOrder)
				.Select(o => o.OptionName)
				.ToList();

			return new CategorySpecDto
			{
				Id = attr.Id,
				Name = attr.Name,
				InputType = attr.InputType,
				IsRequired = attr.IsRequired,
				SortOrder = attr.SortOrder,
				Options = options
			};
		}

		/// <summary>
		/// 新增規格主表，並同步寫入選項子表
		/// </summary>
		public void Create(string name, string inputType, bool isRequired, int sortOrder, List<string> options)
		{
			// 建立 Attributes 主表紀錄
			var newAttr = new ProductAttribute // 若與 C# 保留字衝突請用完整命名空間
			{
				Name = name,
				InputType = inputType,
				IsRequired = isRequired,
				SortOrder = sortOrder
			};
			_db.Attributes.Add(newAttr);
			_db.SaveChanges(); // 先儲存以取得新產生的 Id

			// 若有選項，寫入 AttributeOptions 子表
			WriteOptions(newAttr.Id, options);
			_db.SaveChanges();
		}

		/// <summary>
		/// 更新規格主表，並先清除再重寫選項子表（以新列表為準）
		/// </summary>
		public void Update(int id, string name, string inputType, bool isRequired, int sortOrder, List<string> options)
		{
			var attr = _db.Attributes.FirstOrDefault(a => a.Id == id);
			if (attr == null) return;

			// 更新主表欄位
			attr.Name = name;
			attr.InputType = inputType;
			attr.IsRequired = isRequired;
			attr.SortOrder = sortOrder;

			// 刪除此規格的舊選項（先刪全部，再重新寫入前端送來的新選項）
			var oldOptions = _db.AttributeOptions.Where(o => o.AttributeId == id).ToList();
			_db.AttributeOptions.RemoveRange(oldOptions);

			// 重新寫入新選項
			WriteOptions(id, options);

			_db.SaveChanges();
		}

		/// <summary>
		/// 刪除規格主表，連動刪除 AttributeOptions 與 CategoryAttributes 關聯
		/// </summary>
		public void Delete(int id)
		{
			// 刪除 CategoryAttributes 關聯（避免外鍵錯誤）
			var categoryLinks = _db.CategoryAttributes.Where(ca => ca.AttributeId == id).ToList();
			_db.CategoryAttributes.RemoveRange(categoryLinks);

			// 刪除 AttributeOptions 子表
			var opts = _db.AttributeOptions.Where(o => o.AttributeId == id).ToList();
			_db.AttributeOptions.RemoveRange(opts);

			// 刪除 Attributes 主表
			var attr = _db.Attributes.FirstOrDefault(a => a.Id == id);
			if (attr != null)
			{
				_db.Attributes.Remove(attr);
			}

			_db.SaveChanges();
		}

		/// <summary>
		/// 取得全部分類（含 ParentId，依排序欄位升序）
		/// </summary>
		public IEnumerable<CategoryDto> GetAllCategories()
		{
			return _db.Categories
				.OrderBy(c => c.Sort)
				.ThenBy(c => c.Name)
				.Select(c => new CategoryDto
				{
					Id       = c.Id,
					Name     = c.Name,
					ParentId = c.ParentId
				})
				.ToList();
		}

		/// <summary>
		/// 取得分類名稱
		/// </summary>
		public string GetCategoryName(int categoryId)
		{
			return _db.Categories
				.Where(c => c.Id == categoryId)
				.Select(c => c.Name)
				.FirstOrDefault() ?? "（未知分類）";
		}

		/// <summary>
		/// 取得某分類所有規格的綁定狀態，並包含選項預覽（最多 5 個）
		/// </summary>
		public IEnumerable<CategorySpecBindingDto> GetSpecsForCategory(int categoryId)
		{
			// 取得此分類已綁定的規格 ID 集合
			var boundIds = _db.CategoryAttributes
				.Where(ca => ca.CategoryId == categoryId)
				.Select(ca => ca.AttributeId)
				.ToHashSet();

			// 取得全部規格（依全域 SortOrder）
			var allAttributes = _db.Attributes
				.OrderBy(a => a.SortOrder)
				.ToList();

			// 一次性撈出所有選項（避免 N+1）
			var allOptions = _db.AttributeOptions.ToList();

			return allAttributes.Select(a => new CategorySpecBindingDto
			{
				AttributeId  = a.Id,
				AttributeName = a.Name,
				InputType    = a.InputType,
				IsRequired   = a.IsRequired,
				SortOrder    = a.SortOrder,
				IsBound      = boundIds.Contains(a.Id),
				OptionPreview = allOptions
					.Where(o => o.AttributeId == a.Id)
					.OrderBy(o => o.SortOrder)
					.Take(5)
					.Select(o => o.OptionName)
					.ToList()
			}).ToList();
		}

		/// <summary>
		/// 取得某分類已綁定的規格 ID 清單
		/// </summary>
		public List<int> GetBoundAttributeIds(int categoryId)
		{
			return _db.CategoryAttributes
				.Where(ca => ca.CategoryId == categoryId)
				.Select(ca => ca.AttributeId)
				.ToList();
		}

		/// <summary>
		/// 更新某分類的規格綁定（先刪後寫，以傳入的 attributeIds 為準）
		/// </summary>
		public void UpdateSpecBindingsForCategory(int categoryId, List<int> attributeIds)
		{
			// 刪除此分類所有舊綁定
			var existing = _db.CategoryAttributes
				.Where(ca => ca.CategoryId == categoryId)
				.ToList();
			_db.CategoryAttributes.RemoveRange(existing);

			// 寫入新綁定
			if (attributeIds != null && attributeIds.Count > 0)
			{
				var newBindings = attributeIds.Select(attrId => new CategoryAttribute
				{
					CategoryId  = categoryId,
					AttributeId = attrId,
					IsFilterable = false   // 預設關閉，可日後擴充
				});
				_db.CategoryAttributes.AddRange(newBindings);
			}

			_db.SaveChanges();
		}

		/// <summary>
		/// 儲存某分類的規格綁定（先刪除舊綁定，再重新寫入）
		/// 注意：CategoryAttribute EF Model 無 SortOrder 欄位，以 IsFilterable = false 代替
		/// </summary>
		public void SaveCategoryBindings(int categoryId, List<int> attributeIds)
		{
			// 先刪除此分類的所有綁定
			var existing = _db.CategoryAttributes
				.Where(ca => ca.CategoryId == categoryId)
				.ToList();
			_db.CategoryAttributes.RemoveRange(existing);

			// 重新寫入
			if (attributeIds != null && attributeIds.Count > 0)
			{
				var newBindings = attributeIds.Select(id => new CategoryAttribute
				{
					CategoryId   = categoryId,
					AttributeId  = id,
					IsFilterable = false
				});
				_db.CategoryAttributes.AddRange(newBindings);
			}

			_db.SaveChanges();
		}

		/// <summary>
		/// 取得全部 CategoryAttributes 綁定紀錄（供矩陣頁一次性載入）
		/// </summary>
		public IEnumerable<(int CategoryId, int AttributeId)> GetAllBindings()
		{
			return _db.CategoryAttributes
				.Select(ca => new { ca.CategoryId, ca.AttributeId })
				.AsEnumerable()
				.Select(x => (x.CategoryId, x.AttributeId))
				.ToList();
		}

		/// <summary>
		/// 切換單一規格綁定（isBound=true 新增，false 刪除），避免全表重建
		/// </summary>
		public void ToggleSpec(int categoryId, int attributeId, bool isBound)
		{
			var existing = _db.CategoryAttributes
				.FirstOrDefault(ca => ca.CategoryId == categoryId && ca.AttributeId == attributeId);

			if (isBound && existing == null)
			{
				_db.CategoryAttributes.Add(new CategoryAttribute
				{
					CategoryId   = categoryId,
					AttributeId  = attributeId,
					IsFilterable = false
				});
			}
			else if (!isBound && existing != null)
			{
				_db.CategoryAttributes.Remove(existing);
			}

			_db.SaveChanges();
		}

		// ─────────────────────────────────────────────
		// 私有輔助方法
		// ─────────────────────────────────────────────

		private void WriteOptions(int attributeId, List<string> options)
		{
			if (options == null || options.Count == 0) return;

			var validOptions = options
				.Where(o => !string.IsNullOrWhiteSpace(o))
				.Select((optionName, index) => new AttributeOption
				{
					AttributeId = attributeId,
					OptionName  = optionName.Trim(),
					SortOrder   = index + 1
				});

			_db.AttributeOptions.AddRange(validOptions);
		}
	}
}
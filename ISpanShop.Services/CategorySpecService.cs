using System.Collections.Generic;
using ISpanShop.Models.DTOs;
using ISpanShop.Repositories.Interfaces;

namespace ISpanShop.Services
{
	public class CategorySpecService
	{
		private readonly ICategorySpecRepository _categorySpecRepository;

		public CategorySpecService(ICategorySpecRepository categorySpecRepository)
		{
			_categorySpecRepository = categorySpecRepository;
		}

		public IEnumerable<CategorySpecDto> GetAll()
		{
			return _categorySpecRepository.GetAll();
		}

		public CategorySpecDto? GetById(int id)
		{
			return _categorySpecRepository.GetById(id);
		}

		/// <summary>
		/// 新增分類規格 - 接受基本型別參數，不依賴 MVC 的 ViewModel
		/// </summary>
		public void Create(string name, string inputType, bool isRequired, int sortOrder, List<string> options)
		{
			var cleanOptions = NeedsOptions(inputType) ? options : new List<string>();
			_categorySpecRepository.Create(name, inputType, isRequired, sortOrder, cleanOptions);
		}

		/// <summary>
		/// 更新分類規格 - 接受基本型別參數，不依賴 MVC 的 ViewModel
		/// </summary>
		public void Update(int id, string name, string inputType, bool isRequired, int sortOrder, List<string> options)
		{
			var cleanOptions = NeedsOptions(inputType) ? options : new List<string>();
			_categorySpecRepository.Update(id, name, inputType, isRequired, sortOrder, cleanOptions);
		}

		public void Delete(int id)
		{
			_categorySpecRepository.Delete(id);
		}

		// ── 分類綁定相關 ──────────────────────────────

		/// <summary>
		/// 取得全部分類（供左側樹狀列表渲染）
		/// </summary>
		public IEnumerable<CategoryDto> GetAllCategories()
		{
			return _categorySpecRepository.GetAllCategories();
		}

		/// <summary>
		/// 取得分類名稱
		/// </summary>
		public string GetCategoryName(int categoryId)
		{
			return _categorySpecRepository.GetCategoryName(categoryId);
		}

		/// <summary>
		/// 取得某分類的全部規格綁定狀態（含選項預覽），供 ManageBindings 頁使用
		/// </summary>
		public IEnumerable<CategorySpecBindingDto> GetSpecsForCategory(int categoryId)
		{
			return _categorySpecRepository.GetSpecsForCategory(categoryId);
		}

		/// <summary>
		/// 取得某分類已綁定的規格 ID 清單
		/// </summary>
		public List<int> GetBoundAttributeIds(int categoryId)
		{
			return _categorySpecRepository.GetBoundAttributeIds(categoryId);
		}

		/// <summary>
		/// 更新某分類的規格綁定（先刪後寫）
		/// </summary>
		public void UpdateSpecBindingsForCategory(int categoryId, List<int> attributeIds)
		{
			_categorySpecRepository.UpdateSpecBindingsForCategory(categoryId, attributeIds);
		}

		/// <summary>
		/// 儲存某分類的規格綁定（語意別名，供 CategoryBindingController 使用）
		/// </summary>
		public void SaveCategoryBindings(int categoryId, List<int> attributeIds)
		{
			_categorySpecRepository.SaveCategoryBindings(categoryId, attributeIds);
		}

		/// <summary>
		/// 取得全部 CategoryAttributes 綁定紀錄（供矩陣頁一次性載入）
		/// </summary>
		public IEnumerable<(int CategoryId, int AttributeId)> GetAllBindings()
		{
			return _categorySpecRepository.GetAllBindings();
		}

		/// <summary>
		/// 切換單一規格綁定
		/// </summary>
		public void ToggleSpec(int categoryId, int attributeId, bool isBound)
		{
			_categorySpecRepository.ToggleSpec(categoryId, attributeId, isBound);
		}

		private static bool NeedsOptions(string inputType)
		{
			return inputType == "select"
				|| inputType == "checkbox"
				|| inputType == "radio";
		}
	}
}
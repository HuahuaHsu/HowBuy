using System.Collections.Generic;
using ISpanShop.Models.DTOs;

namespace ISpanShop.Repositories.Interfaces
{
	/// <summary>
	/// 分類規格 Repository 介面
	/// 定義對 Attributes 與 AttributeOptions 資料表的存取契約
	/// </summary>
	public interface ICategorySpecRepository
	{
		/// <summary>
		/// 取得所有分類規格（含對應選項列表）
		/// </summary>
		IEnumerable<CategorySpecDto> GetAll();

		/// <summary>
		/// 根據 ID 取得單一分類規格（含對應選項列表）
		/// </summary>
		CategorySpecDto? GetById(int id);

		/// <summary>
		/// 新增分類規格，同時寫入 Attributes 主表與 AttributeOptions 選項子表
		/// </summary>
		void Create(string name, string inputType, bool isRequired, int sortOrder, List<string> options);

		/// <summary>
		/// 更新分類規格，同時先清除再重寫 AttributeOptions 選項子表
		/// </summary>
		void Update(int id, string name, string inputType, bool isRequired, int sortOrder, List<string> options);

		/// <summary>
		/// 刪除分類規格，連動刪除 AttributeOptions 與 CategoryAttributes 的關聯資料
		/// </summary>
		void Delete(int id);

		/// <summary>
		/// 取得全部分類（含 ParentId，供左側樹狀列表渲染）
		/// </summary>
		IEnumerable<CategoryDto> GetAllCategories();

		/// <summary>
		/// 取得分類名稱
		/// </summary>
		string GetCategoryName(int categoryId);

		/// <summary>
		/// 取得某分類已綁定的規格列表（含規格選項預覽，最多 5 個）
		/// 回傳全部規格並標記 IsBound，供 ManageBindings 頁面使用
		/// </summary>
		IEnumerable<CategorySpecBindingDto> GetSpecsForCategory(int categoryId);

		/// <summary>
		/// 取得某分類已綁定的規格 ID 清單
		/// </summary>
		List<int> GetBoundAttributeIds(int categoryId);

		/// <summary>
		/// 更新某分類的規格綁定（先刪後寫）
		/// </summary>
		void UpdateSpecBindingsForCategory(int categoryId, List<int> attributeIds);

		/// <summary>
		/// 儲存某分類的規格綁定（UpdateSpecBindingsForCategory 的語意別名）
		/// </summary>
		void SaveCategoryBindings(int categoryId, List<int> attributeIds);

		/// <summary>
		/// 取得全部 CategoryAttributes 綁定紀錄（供矩陣頁一次性載入）
		/// </summary>
		IEnumerable<(int CategoryId, int AttributeId)> GetAllBindings();

		/// <summary>
		/// 切換單一規格綁定（isBound=true 新增，false 刪除），無需重建全部
		/// </summary>
		void ToggleSpec(int categoryId, int attributeId, bool isBound);
	}
}
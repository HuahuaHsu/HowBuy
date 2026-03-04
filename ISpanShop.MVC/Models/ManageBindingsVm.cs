using System.Collections.Generic;

namespace ISpanShop.MVC.Models
{
	/// <summary>
	/// 管理分類規格綁定頁面的 ViewModel
	/// </summary>
	public class ManageBindingsVm
	{
		/// <summary>規格 ID</summary>
		public int AttributeId { get; set; }

		/// <summary>規格名稱（顯示用）</summary>
		public string AttributeName { get; set; } = string.Empty;

		/// <summary>所有分類的綁定狀態列表</summary>
		public List<CategoryBindingItem> Categories { get; set; } = new();
	}

	/// <summary>
	/// 單一分類的綁定資訊
	/// </summary>
	public class CategoryBindingItem
	{
		/// <summary>分類 ID</summary>
		public int CategoryId { get; set; }

		/// <summary>分類名稱</summary>
		public string CategoryName { get; set; } = string.Empty;

		/// <summary>是否已綁定此規格</summary>
		public bool IsBound { get; set; }

		/// <summary>是否可在前台篩選</summary>
		public bool IsFilterable { get; set; }
	}
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISpanShop.Models.ViewModels
{
	/// <summary>
	/// 新增分類規格用 ViewModel - 接收前端表單資料
	/// </summary>
	public class CategorySpecCreateVm
	{
		/// <summary>
		/// 規格名稱（例如：顏色、尺寸、容量）- 必填
		/// </summary>
		[Required(ErrorMessage = "規格名稱為必填")]
		[StringLength(100, ErrorMessage = "規格名稱不可超過 100 字")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// 輸入方式：text=文字輸入, select=下拉選單, checkbox=多選框, radio=單選框
		/// </summary>
		[Required(ErrorMessage = "請選擇輸入方式")]
		public string InputType { get; set; } = "text";

		/// <summary>
		/// 是否為必填項目
		/// </summary>
		public bool IsRequired { get; set; } = false;

		/// <summary>
		/// 排序順序（數字越小越前面）
		/// </summary>
		public int SortOrder { get; set; } = 0;

		/// <summary>
		/// 選項列表 - 當 InputType 為 select/checkbox/radio 時需填入
		/// 對應 AttributeOptions 資料表的 OptionName 欄位
		/// </summary>
		public List<string> Options { get; set; } = new List<string>();
	}
}
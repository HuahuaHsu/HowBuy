using System.ComponentModel.DataAnnotations;

namespace ISpanShop.MVC.Models.ViewModels
{
	public class SensitiveWordVm
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "請輸入敏感字內容")]
		[StringLength(100, ErrorMessage = "長度不能超過 100 個字元")]
		[Display(Name = "敏感字內容")]
		public string Word { get; set; }

		[Required(ErrorMessage = "請選擇分類")]
		[Display(Name = "分類")]
		public string Category { get; set; }

		[Display(Name = "是否啟用")]
		public bool IsActive { get; set; }
	}
}
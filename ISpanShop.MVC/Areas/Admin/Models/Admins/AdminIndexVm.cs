using ISpanShop.Models.DTOs.Admins;
using System.Collections.Generic;

namespace ISpanShop.MVC.Areas.Admin.Models.Admins
{
	/// <summary>
	/// 管理員列表 ViewModel
	/// </summary>
	public class AdminIndexVm
	{
		/// <summary>
		/// 管理員列表
		/// </summary>
		public List<AdminDto> Admins { get; set; } = new List<AdminDto>();

		/// <summary>
		/// 新增管理員表單
		/// </summary>
		public AdminCreateVm CreateForm { get; set; } = new AdminCreateVm();

		/// <summary>
		/// 操作結果訊息（成功/失敗）
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 系統預產的下一個帳號（Modal 唯讀顯示用）
		/// </summary>
		public string NextAccount { get; set; } = "";

	}
}


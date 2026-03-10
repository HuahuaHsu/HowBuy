using ISpanShop.Models.DTOs;
using System.Collections.Generic;

namespace ISpanShop.MVC.Models.LoginHistories
{
	/// <summary>
	/// 登入紀錄列表 ViewModel - 用於呈現登入紀錄的後台管理界面
	/// </summary>
	public class LoginHistoryIndexVm
	{
		/// <summary>
		/// 登入紀錄列表
		/// </summary>
		public List<LoginHistoryItemVm> LoginHistories { get; set; } = new List<LoginHistoryItemVm>();

		/// <summary>
		/// 操作結果訊息（成功/失敗）
		/// </summary>
		public string Message { get; set; }
	}

	/// <summary>
	/// 登入紀錄項目 ViewModel - 用於在列表中顯示單筆登入紀錄
	/// </summary>
	public class LoginHistoryItemVm
	{
		/// <summary>
		/// 紀錄ID
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// 使用者帳號
		/// </summary>
		public string UserAccount { get; set; }

		/// <summary>
		/// 登入時間（格式化為字串）
		/// </summary>
		public string LoginTimeFormatted { get; set; }

		/// <summary>
		/// IP位址
		/// </summary>
		public string IpAddress { get; set; }

		/// <summary>
		/// 登入狀態文字（成功/失敗/未知）
		/// </summary>
		public string StatusText { get; set; }

		/// <summary>
		/// 登入狀態 CSS 類別（用於 Badge 樣式）
		/// </summary>
		public string StatusClass { get; set; }

		/// <summary>
		/// 原始的成功/失敗布林值
		/// </summary>
		public bool? IsSuccessful { get; set; }
	}

	/// <summary>
	/// DTO 轉 ViewModel 的轉換擴充方法
	/// </summary>
	public static class LoginHistoryMappingExtensions
	{
		/// <summary>
		/// 將 LoginHistoryDto 轉換為 LoginHistoryItemVm
		/// </summary>
		public static LoginHistoryItemVm ToViewModel(this LoginHistoryDto dto)
		{
			return new LoginHistoryItemVm
			{
				Id = dto.Id,
				UserAccount = dto.UserAccount,
				LoginTimeFormatted = dto.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-",
				IpAddress = string.IsNullOrEmpty(dto.Ipaddress) ? "-" : dto.Ipaddress,
				IsSuccessful = dto.IsSuccessful,
				StatusText = GetStatusText(dto.IsSuccessful),
				StatusClass = GetStatusClass(dto.IsSuccessful)
			};
		}

		/// <summary>
		/// 取得登入狀態文字
		/// </summary>
		private static string GetStatusText(bool? isSuccessful)
		{
			return isSuccessful switch
			{
				true => "登入成功",
				false => "登入失敗",
				_ => "未知"
			};
		}

		/// <summary>
		/// 取得登入狀態的 CSS 類別
		/// </summary>
		private static string GetStatusClass(bool? isSuccessful)
		{
			return isSuccessful switch
			{
				true => "bg-green-100 text-green-800",
				false => "bg-red-100 text-red-800",
				_ => "bg-gray-100 text-gray-800"
			};
		}
	}
}

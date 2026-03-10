using ISpanShop.Models.DTOs;
using System.Collections.Generic;

namespace ISpanShop.Services.Interfaces
{
	/// <summary>
	/// 登入紀錄服務介面 - 處理登入紀錄相關業務邏輯
	/// </summary>
	public interface ILoginHistoryService
	{
		/// <summary>
		/// 取得所有登入紀錄
		/// </summary>
		/// <returns>登入紀錄 DTO 列表</returns>
		IEnumerable<LoginHistoryDto> GetAllLoginHistories();
	}
}

using ISpanShop.Models.DTOs;
using System.Collections.Generic;

namespace ISpanShop.Repositories.Interfaces
{
	/// <summary>
	/// 登入紀錄 Repository 介面
	/// </summary>
	public interface ILoginHistoryRepository
	{
		/// <summary>
		/// 取得所有登入紀錄（含使用者帳號，由新至舊排序）
		/// </summary>
		/// <returns>登入紀錄 DTO 列表</returns>
		IEnumerable<LoginHistoryDto> GetAll();
	}
}

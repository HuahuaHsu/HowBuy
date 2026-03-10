using ISpanShop.Models.DTOs;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ISpanShop.Repositories
{
	/// <summary>
	/// 登入紀錄 Repository 實作 - 負責資料庫查詢
	/// </summary>
	public class LoginHistoryRepository : ILoginHistoryRepository
	{
		private readonly ISpanShopDBContext _context;

		public LoginHistoryRepository(ISpanShopDBContext context)
		{
			_context = context;
		}

		public IEnumerable<LoginHistoryDto> GetAll()
		{
			return _context.LoginHistories
				.AsNoTracking()
				.Include(lh => lh.User)
				.OrderByDescending(lh => lh.LoginTime)
				.Select(lh => new LoginHistoryDto
				{
					Id = lh.Id,
					UserId = lh.UserId,
					UserAccount = lh.User.Account,
					LoginTime = lh.LoginTime,
					Ipaddress = lh.Ipaddress,
					IsSuccessful = lh.IsSuccessful
				})
				.ToList();
		}
	}
}

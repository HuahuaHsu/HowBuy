using ISpanShop.MVC.Models.LoginHistories;
using ISpanShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.MVC.Controllers
{
	/// <summary>
	/// 登入紀錄管理 Controller - 處理登入紀錄的後台管理
	/// </summary>
	public class LoginHistoryController : Controller
	{
		private readonly ILoginHistoryService _loginHistoryService;

		public LoginHistoryController(ILoginHistoryService loginHistoryService)
		{
			_loginHistoryService = loginHistoryService ?? throw new ArgumentNullException(nameof(loginHistoryService));
		}

		/// <summary>
		/// 登入紀錄列表頁 - 顯示所有登入紀錄
		/// </summary>
		public IActionResult Index()
		{
			try
			{
				var loginHistories = _loginHistoryService.GetAllLoginHistories()
					.Select(lh => lh.ToViewModel())
					.ToList();

				var viewModel = new LoginHistoryIndexVm
				{
					LoginHistories = loginHistories,
					Message = TempData["Message"]?.ToString()
				};

				return View(viewModel);
			}
			catch (Exception ex)
			{
				var emptyVm = new LoginHistoryIndexVm
				{
					LoginHistories = new List<LoginHistoryItemVm>(),
					Message = $"載入登入紀錄失敗: {ex.Message}"
				};
				return View(emptyVm);
			}
		}
	}
}

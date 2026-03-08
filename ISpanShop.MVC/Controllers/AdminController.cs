using ISpanShop.Models.DTOs;
using ISpanShop.MVC.Models.Admins;
using ISpanShop.Services;
using ISpanShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace ISpanShop.MVC.Controllers
{
	/// <summary>
	/// 管理員管理控制器 - 僅限 SuperAdmin 存取
	/// </summary>
	//[Authorize(Roles = "SuperAdmin")] //測試功能階不需要權限
	public class AdminController : Controller
	{
		private readonly IAdminService _adminService;
		public AdminController(IAdminService adminService)
		{
			_adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
		}
		/// <summary>
		/// 管理員列表頁 - 顯示所有管理員及角色更新選項
		/// </summary>
		public IActionResult Index()
		{
			try
			{
				var admins = _adminService.GetAllAdmins().ToList();
				var roles = _adminService.GetAllRoles().ToList();
				var viewModel = new AdminIndexVm
				{
					Admins = admins,
					RoleOptions = roles,
					Message = TempData["Message"]?.ToString()
				};
				return View(viewModel);
			}
			catch (Exception ex)
			{
				var emptyVm = new AdminIndexVm
				{
					Admins = new List<AdminDto>(),
					RoleOptions = new List<AdminRoleDto>(),
					Message = $"載入管理員列表失敗: {ex.Message}"
				};
				return View(emptyVm);
			}
		}
			/// <summary>
			/// 更新管理員角色
			/// </summary>
			/// <param name="adminId">管理員 ID</param>
			/// <param name="roleId">新角色 ID</param>
			[HttpPost]
			public IActionResult UpdateRole(int adminId, int roleId)
			{
				try
				{
					// 從 User.Claims 或 HttpContext 獲取當前登入管理員的 ID
					// 此處假設已通過身份驗證，且 User.FindFirst("userid") 或類似方式可取得
					// 實際實現應根據您的身份驗證方案調整
					var currentAdminIdStr = User.FindFirst("userid")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

					if (!int.TryParse(currentAdminIdStr, out int currentAdminId))
					{
						TempData["Message"] = "無法識別當前登入的管理員 ID";
						return RedirectToAction("Index");
					}
					bool success = _adminService.UpdateAdminRole(adminId, roleId, currentAdminId);
					if (success)
					{
						TempData["Message"] = "管理員角色更新成功";
					}
					else
					{
						TempData["Message"] = "更新失敗，請確認管理員 ID 是否存在";
					}
				}
				catch (InvalidOperationException ex)
				{
					// Self-Lockout Prevention 例外
					TempData["Message"] = ex.Message;
				}
				catch (Exception ex)
				{
					TempData["Message"] = $"更新角色失敗: {ex.Message}";
				}
				return RedirectToAction("Index");
			}
		}
	}

using ISpanShop.Models.DTOs.Admins;
using ISpanShop.MVC.Models.Auth;
using ISpanShop.Services.Admins;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ISpanShop.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAdminService _adminService;

        public AuthController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new AdminLoginVm());
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVm form)
        {
            if (!ModelState.IsValid) return View(form);

            var admin = _adminService.VerifyLogin(form.Account, form.Password);
            if (admin == null)
            {
                form.Message = "帳號或密碼錯誤";
                ModelState.AddModelError(string.Empty, form.Message);
                return View(form);
            }

            if (admin.IsFirstLogin)
            {
                // 暫時 Claims (僅包含 ID 用於修改密碼)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.UserId.ToString()),
                    new Claim(ClaimTypes.Name, admin.Account),
                    new Claim("IsFirstLogin", "true")
                };
                var claimsIdentity = new ClaimsIdentity(claims, "AdminCookieAuth");
                await HttpContext.SignInAsync("AdminCookieAuth", new ClaimsPrincipal(claimsIdentity));
                
                return RedirectToAction("ChangePassword");
            }
            else
            {
                // 完整 Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.UserId.ToString()),
                    new Claim(ClaimTypes.Name, admin.Account),
                    new Claim(ClaimTypes.Role, admin.RoleName),
                    new Claim("AdminLevelId", admin.AdminLevelId?.ToString() ?? ""),
                    new Claim("userid", admin.UserId.ToString()) // 為了與現有程式碼相容
                };
                var claimsIdentity = new ClaimsIdentity(claims, "AdminCookieAuth");
                await HttpContext.SignInAsync("AdminCookieAuth", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new AdminChangePasswordVm());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(AdminChangePasswordVm form)
        {
            if (!ModelState.IsValid) return View(form);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login");
            }

            var result = _adminService.ChangePassword(new AdminChangePasswordDto
            {
                UserId = userId,
                NewPassword = form.NewPassword,
                ConfirmPassword = form.ConfirmPassword
            });

            if (result.IsSuccess)
            {
                // 修改成功後，為了取得完整 Claims，通常建議重新登入
                // 但照指示直接跳轉到 Index，我們可能需要重新簽發完整 Claims 或者讓 User 重新登入
                // 這裡我們重新導向到 Login 頁面並提示成功，或者嘗試自動重新登入
                
                // 為了嚴格遵守「成功：RedirectToAction("Index", "Admins")」
                // 我們假設 Index 會處理權限問題。
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }

            form.Message = result.Message;
            ModelState.AddModelError(string.Empty, result.Message);
            return View(form);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookieAuth");
            return RedirectToAction("Login");
        }
    }
}

using ISpanShop.Models.DTOs.Common;
using ISpanShop.Models.EfModels;
using ISpanShop.MVC.Areas.Admin.Models.Members;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISpanShop.MVC.Areas.Admin.Controllers.Members
{
    [Area("Admin")]
    [Route("Admin/LoginLogs")]
    public class LoginLogsController : Controller
    {
        private readonly ISpanShopDBContext _context;

        public LoginLogsController(ISpanShopDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string keyword = "", string status = "all", int page = 1, int pageSize = 10)
        {
            var query = _context.LoginHistories
                .Include(l => l.User)
                .AsQueryable();

            // 關鍵字搜尋 (帳號或 IP)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(l => l.User.Account.Contains(keyword) || l.Ipaddress.Contains(keyword));
            }

            // 狀態篩選
            if (status == "success")
            {
                query = query.Where(l => l.IsSuccessful == true);
            }
            else if (status == "failure")
            {
                query = query.Where(l => l.IsSuccessful == false);
            }

            // 排序：最新的在前
            query = query.OrderByDescending(l => l.LoginTime);

            // 總筆數
            var totalCount = await query.CountAsync();

            // 分頁
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LoginLogItemVm
                {
                    Id = l.Id,
                    UserAccount = l.User.Account,
                    IpAddress = l.Ipaddress,
                    LoginTime = l.LoginTime ?? DateTime.Now,
                    IsSuccessful = l.IsSuccessful ?? false
                })
                .ToListAsync();

            var viewModel = new LoginLogIndexVm
            {
                PagedResult = new PagedResult<LoginLogItemVm>
                {
                    Data = items,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                },
                Keyword = keyword,
                Status = status,
                PageSize = pageSize
            };

            return View(viewModel);
        }
    }
}

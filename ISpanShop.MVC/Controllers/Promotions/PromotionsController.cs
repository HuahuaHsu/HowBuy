using ISpanShop.MVC.Models.Promotions;
using ISpanShop.Models.EfModels;
using ISpanShop.Services.Promotions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISpanShop.MVC.Controllers.Promotions
{
    [Route("Promotions")]
    public class PromotionsController : Controller
    {
        private readonly ISpanShopDBContext _context;
        private readonly PromotionService _promotionService;

        public PromotionsController(ISpanShopDBContext context, PromotionService promotionService)
        {
            _context = context;
            _promotionService = promotionService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string? keyword, string? status, int? type, int page = 1, int pageSize = 10)
        {
            var now = DateTime.Now;
            var query = _context.Promotions
                .Include(p => p.Seller).ThenInclude(u => u.Stores)
                .Include(p => p.PromotionItems).Include(p => p.PromotionRules)
                .Where(p => !p.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || p.Seller.Account.Contains(keyword) || p.Seller.Stores.Any(s => s.StoreName.Contains(keyword)));
            if (type.HasValue)
                query = query.Where(p => p.PromotionType == type.Value);

            query = status switch {
                "pending" => query.Where(p => p.Status == 0),
                "active" => query.Where(p => p.Status == 1 && p.StartTime <= now && p.EndTime >= now),
                "upcoming" => query.Where(p => p.Status == 1 && p.StartTime > now),
                "rejected" => query.Where(p => p.Status == 2),
                "ended" => query.Where(p => p.Status == 1 && p.EndTime < now),
                _ => query
            };

            var totalCount = await query.CountAsync();
            var allPromotions = await _context.Promotions.Where(p => !p.IsDeleted).ToListAsync();
            var promotions = await query.OrderByDescending(p => p.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var vm = new PromotionIndexVm {
                Keyword = keyword, StatusFilter = status, TypeFilter = type,
                TotalCount = totalCount, CurrentPage = page, PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                AllCount = allPromotions.Count,
                PendingCount = allPromotions.Count(p => p.Status == 0),
                ActiveCount = allPromotions.Count(p => p.Status == 1 && p.StartTime <= now && p.EndTime >= now),
                UpcomingCount = allPromotions.Count(p => p.Status == 1 && p.StartTime > now),
                EndedCount = allPromotions.Count(p => p.Status == 1 && p.EndTime < now),
                ReSubmittedCount = 0,
                RejectedCount = allPromotions.Count(p => p.Status == 2),
                Items = promotions.Select(p => new PromotionListItemVm {
                    Id = p.Id, Name = p.Name, PromotionType = p.PromotionType,
                    StartTime = p.StartTime, EndTime = p.EndTime, Status = p.Status,
                    SellerName = p.Seller.Stores.FirstOrDefault(s => s.StoreStatus == 1)?.StoreName ?? p.Seller.Account,
                    ItemCount = p.PromotionItems.Count + p.PromotionRules.Count
                }).ToList()
            };
            return View(vm);
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id) {
            var p = await _context.Promotions.Include(x => x.Seller).ThenInclude(u => u.Stores)
                .Include(x => x.PromotionItems).Include(x => x.PromotionRules)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (p == null) { TempData["Error"] = "找不到此活動"; return RedirectToAction(nameof(Index)); }
            return View(p);
        }

        [HttpGet("Create")]
        public IActionResult Create() { TempData["Warning"] = "新增功能開發中"; return RedirectToAction(nameof(Index)); }
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id) { TempData["Warning"] = "編輯功能開發中"; return RedirectToAction(nameof(Index)); }
        [HttpPost("Approve/{id}")]
        public IActionResult Approve(int id) { TempData["Warning"] = "審核功能開發中"; return RedirectToAction(nameof(Index)); }
        [HttpPost("Reject/{id}")]
        public IActionResult Reject(int id, string? reason) { TempData["Warning"] = "拒絕功能開發中"; return RedirectToAction(nameof(Index)); }
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id) { TempData["Warning"] = "刪除功能開發中"; return RedirectToAction(nameof(Index)); }

        [HttpGet("SearchProducts")]
        public IActionResult SearchProducts(string? keyword, int page = 1, int pageSize = 5) {
            var query = _context.Products.Include(p => p.Category).Include(p => p.ProductImages)
                .Where(p => !p.IsDeleted && p.Status == 1).AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword)) query = query.Where(p => p.Name.Contains(keyword));
            int totalCount = query.Count(), totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).Select(p => new {
                p.Id, p.Name, Price = p.MinPrice ?? 0,
                CategoryName = p.Category != null ? p.Category.Name : "未分類",
                ImageUrl = p.ProductImages.FirstOrDefault(img => img.IsMain == true) != null ? 
                    p.ProductImages.FirstOrDefault(img => img.IsMain == true).ImageUrl : "https://via.placeholder.com/60"
            }).ToList();
            return Json(new { items, totalCount, totalPages, currentPage = page });
        }
    }
}

using ISpanShop.Models.EfModels;
using ISpanShop.MVC.Areas.Admin.Controllers;
using ISpanShop.MVC.Models.Coupons;
using ISpanShop.Services.Coupons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISpanShop.MVC.Areas.Admin.Controllers.Coupons
{
    [Area("Admin")]
    public class CouponsController : AdminBaseController
    {
        private readonly ICouponService _couponService;
        private readonly ISpanShopDBContext _context;

        public CouponsController(ICouponService couponService, ISpanShopDBContext context)
        {
            _couponService = couponService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _context.Coupons
                .Include(c => c.Store)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();

            var vm = new CouponIndexVm
            {
                Items = coupons.Select(c => new CouponListItemVm
                {
                    Id = c.Id,
                    Title = c.Title,
                    CouponCode = c.CouponCode,
                    // 只要 ApplyToAll 是 true，列表就顯示全站通用
                    StoreName = c.ApplyToAll ? "--- 全站通用 ---" : (c.Store?.StoreName ?? "未知商家"),
                    DistributionType = c.DistributionType,
                    CouponType = c.CouponType,
                    DiscountValue = c.DiscountValue,
                    MinimumSpend = c.MinimumSpend,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TotalQuantity = c.TotalQuantity,
                    UsedQuantity = c.UsedQuantity
                }).ToList()
            };

            return View(vm);
        }

        private void PrepareStoreList(int? selectedId = null)
        {
            var stores = _context.Stores.Where(s => s.StoreStatus == 1).ToList();
            var storeList = stores.Select(s => new { Id = s.Id, Name = s.StoreName }).ToList();
            // ID 為 0 代表全站通用
            storeList.Insert(0, new { Id = 0, Name = "--- 全站通用 ---" });
            ViewBag.Stores = new SelectList(storeList, "Id", "Name", selectedId);
        }

        public IActionResult Create()
        {
            PrepareStoreList();
            return View(new CouponFormVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponFormVm vm)
        {
            if (ModelState.IsValid)
            {
                if (await _couponService.IsCouponCodeExistsAsync(vm.CouponCode))
                {
                    ModelState.AddModelError("CouponCode", "優惠碼已存在");
                    PrepareStoreList(vm.StoreId);
                    return View(vm);
                }

                var store = vm.StoreId == 0 ? await _context.Stores.FirstOrDefaultAsync() : await _context.Stores.FindAsync(vm.StoreId);
                if (store == null)
                {
                    ModelState.AddModelError("", "找不到指定的商家");
                    PrepareStoreList(vm.StoreId);
                    return View(vm);
                }
                
                // 獲取目前登入者 ID
                var adminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int adminId = string.IsNullOrEmpty(adminIdStr) ? store.UserId : int.Parse(adminIdStr);

                var coupon = new Coupon
                {
                    StoreId = store.Id,
                    SellerId = store.UserId,
                    ApplyToAll = vm.StoreId == 0 ? true : vm.ApplyToAll,
                    Title = vm.Title,
                    CouponCode = vm.CouponCode,
                    DistributionType = vm.DistributionType,
                    CouponType = vm.CouponType,
                    DiscountValue = vm.DiscountValue,
                    MinimumSpend = vm.MinimumSpend,
                    MaximumDiscount = vm.MaximumDiscount,
                    StartTime = vm.StartTime,
                    EndTime = vm.EndTime,
                    TotalQuantity = vm.TotalQuantity,
                    PerUserLimit = vm.PerUserLimit,
                    Status = 1,
                    
                    // 僅保留 DB 確定的欄位
                    UpdatedBy = adminId,
                    UpdatedAt = DateTime.Now
                };

                await _couponService.CreateCouponAsync(coupon);
                return RedirectToAction(nameof(Index));
            }

            PrepareStoreList(vm.StoreId);
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound();

            var vm = new CouponFormVm
            {
                Id = coupon.Id,
                StoreId = coupon.ApplyToAll ? 0 : coupon.StoreId,
                Title = coupon.Title,
                CouponCode = coupon.CouponCode,
                DistributionType = coupon.DistributionType,
                CouponType = coupon.CouponType,
                DiscountValue = coupon.DiscountValue,
                MinimumSpend = coupon.MinimumSpend,
                MaximumDiscount = coupon.MaximumDiscount,
                StartTime = coupon.StartTime,
                EndTime = coupon.EndTime,
                TotalQuantity = coupon.TotalQuantity,
                PerUserLimit = coupon.PerUserLimit,
                ApplyToAll = coupon.ApplyToAll
            };

            PrepareStoreList(vm.StoreId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CouponFormVm vm)
        {
            if (ModelState.IsValid)
            {
                if (await _couponService.IsCouponCodeExistsAsync(vm.CouponCode, vm.Id))
                {
                    ModelState.AddModelError("CouponCode", "優惠碼已存在");
                    PrepareStoreList(vm.StoreId);
                    return View(vm);
                }

                var coupon = await _couponService.GetCouponByIdAsync(vm.Id);
                if (coupon == null) return NotFound();

                var store = vm.StoreId == 0 ? await _context.Stores.FirstOrDefaultAsync() : await _context.Stores.FindAsync(vm.StoreId);
                if (store == null)
                {
                    ModelState.AddModelError("", "找不到指定的商家");
                    PrepareStoreList(vm.StoreId);
                    return View(vm);
                }
                
                // 獲取目前登入者 ID
                var adminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int adminId = string.IsNullOrEmpty(adminIdStr) ? store.UserId : int.Parse(adminIdStr);

                coupon.StoreId = store.Id;
                coupon.SellerId = store.UserId;
                coupon.ApplyToAll = vm.StoreId == 0 ? true : vm.ApplyToAll;
                
                coupon.Title = vm.Title;
                coupon.CouponCode = vm.CouponCode;
                coupon.DistributionType = vm.DistributionType;
                coupon.CouponType = vm.CouponType;
                coupon.DiscountValue = vm.DiscountValue;
                coupon.MinimumSpend = vm.MinimumSpend;
                coupon.MaximumDiscount = vm.MaximumDiscount;
                coupon.StartTime = vm.StartTime;
                coupon.EndTime = vm.EndTime;
                coupon.TotalQuantity = vm.TotalQuantity;
                coupon.PerUserLimit = vm.PerUserLimit;
                
                // 更新審計欄位
                coupon.UpdatedBy = adminId;
                coupon.UpdatedAt = DateTime.Now;

                await _couponService.UpdateCouponAsync(coupon);
                return RedirectToAction(nameof(Index));
            }

            PrepareStoreList(vm.StoreId);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _couponService.DeleteCouponAsync(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Seed()
        {
            var store = await _context.Stores.FirstOrDefaultAsync();
            if (store == null) return Json(new { success = false, message = "找不到商店資料" });

            var adminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int adminId = string.IsNullOrEmpty(adminIdStr) ? store.UserId : int.Parse(adminIdStr);

            var now = DateTime.Now;
            // 使用隨機後置碼確保每次生成都是唯一的
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            var seedCoupons = new List<Coupon>
            {
                new Coupon
                {
                    Title = $"全站開幕大禮包_{suffix}",
                    CouponCode = $"WELCOME88_{suffix}",
                    ApplyToAll = true,
                    StoreId = store.Id,
                    SellerId = store.UserId,
                    DistributionType = 1,
                    CouponType = 1,
                    DiscountValue = 100,
                    MinimumSpend = 500,
                    StartTime = now.AddDays(-1),
                    EndTime = now.AddDays(30),
                    TotalQuantity = 1000,
                    PerUserLimit = 1,
                    Status = 1,
                    UpdatedAt = now,
                    UpdatedBy = adminId
                },
                new Coupon
                {
                    Title = $"滿千折百限時送_{suffix}",
                    CouponCode = $"SAVE100_{suffix}",
                    ApplyToAll = true,
                    StoreId = store.Id,
                    SellerId = store.UserId,
                    DistributionType = 1,
                    CouponType = 1,
                    DiscountValue = 100,
                    MinimumSpend = 1000,
                    StartTime = now.AddDays(-7),
                    EndTime = now.AddDays(7),
                    TotalQuantity = 500,
                    PerUserLimit = 1,
                    Status = 1,
                    UpdatedAt = now,
                    UpdatedBy = adminId
                },
                new Coupon
                {
                    Title = $"九折優惠券_{suffix}",
                    CouponCode = $"OFF90_{suffix}",
                    ApplyToAll = true,
                    StoreId = store.Id,
                    SellerId = store.UserId,
                    DistributionType = 1,
                    CouponType = 2,
                    DiscountValue = 10,
                    MinimumSpend = 0,
                    MaximumDiscount = 200,
                    StartTime = now.AddDays(-2),
                    EndTime = now.AddDays(60),
                    TotalQuantity = 2000,
                    PerUserLimit = 2,
                    Status = 1,
                    UpdatedAt = now,
                    UpdatedBy = adminId
                },
                new Coupon
                {
                    Title = $"新會員專屬禮_{suffix}",
                    CouponCode = $"NEWUSER_{suffix}",
                    ApplyToAll = true,
                    StoreId = store.Id,
                    SellerId = store.UserId,
                    DistributionType = 2,
                    CouponType = 1,
                    DiscountValue = 50,
                    MinimumSpend = 1,
                    StartTime = now.AddDays(-30),
                    EndTime = now.AddDays(365),
                    TotalQuantity = 9999,
                    PerUserLimit = 1,
                    Status = 1,
                    UpdatedAt = now,
                    UpdatedBy = adminId
                }
            };

            _context.Coupons.AddRange(seedCoupons);
            await _context.SaveChangesAsync();
            
            return Json(new { success = true, message = $"成功生成 4 筆新的測試優惠券 (後置碼: {suffix})" });
        }
    }
}

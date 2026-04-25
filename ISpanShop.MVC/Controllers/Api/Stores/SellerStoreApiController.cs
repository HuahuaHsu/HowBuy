using ISpanShop.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.MVC.Controllers.Api.Stores
{
    /// <summary>賣家店鋪自查 API（需 JWT 登入）</summary>
    [ApiController]
    [Route("api/seller/store")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = "FrontendJwt")]
    public class SellerStoreApiController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public SellerStoreApiController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // ──────────────────────────────────────────────────────────
        // GET /api/seller/store/status
        // 賣家查詢自己的店鋪狀態（即使被停權也能看到）
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// 取得賣家自己的店鋪狀態（storeStatus、isBlacklisted 等）
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMyStoreStatus()
        {
            var storeIdClaim = User.FindFirst("StoreId")?.Value
                            ?? User.FindFirst(c => c.Type.EndsWith("/StoreId"))?.Value
                            ?? User.FindFirst(c => c.Type.EndsWith("StoreId"))?.Value;

            if (string.IsNullOrEmpty(storeIdClaim) || !int.TryParse(storeIdClaim, out var storeId))
                return Unauthorized(new { success = false, message = "無法識別賣家身份，請確認您已開通店家" });

            var store = _storeService.GetStoreById(storeId);
            if (store == null)
                return NotFound(new { success = false, message = "找不到店鋪資料" });

            var data = new
            {
                storeId      = store.StoreId,
                storeName    = store.StoreName,
                storeStatus  = store.StoreStatus,           // 1=營業中 2=休假中 3=停權
                storeStatusName = store.StoreStatusName,    // 中文顯示名稱
                isActive     = store.StoreStatus == 1,      // 只有「營業中」視為活躍
                isBanned     = store.IsBlacklisted,         // Users.IsBlacklisted
                bannedReason = (string?)null                // 目前 DB 無此欄位
            };

            return Ok(new { success = true, data, message = "" });
        }
    }
}

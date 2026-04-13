using ISpanShop.Models.DTOs.Products;
using ISpanShop.Services.Products;
using ISpanShop.MVC.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.MVC.Controllers.Api.Products
{
    /// <summary>
    /// 前台商品公開 API（GET 端點不需登入）
    /// </summary>
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsApiController(IProductService productService)
        {
            _productService = productService;
        }

        // ──────────────────────────────────────────────────────────
        // GET /api/products
        // 前台商品總覽，類似蝦皮「每日新發現」商品牆
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// 取得上架中的商品列表（分類篩選、關鍵字搜尋、品牌、價格區間、排序、分頁）
        /// </summary>
        /// <param name="categoryId">主分類篩選（可選，自動展開子分類）</param>
        /// <param name="subCategoryId">子分類篩選（優先於 categoryId，直接篩）</param>
        /// <param name="brandIds">品牌篩選（可傳多個，e.g. ?brandIds=1&amp;brandIds=2）</param>
        /// <param name="minPrice">最低價格（以 MinPrice 比較，可選）</param>
        /// <param name="maxPrice">最高價格（以 MinPrice 比較，可選）</param>
        /// <param name="keyword">關鍵字搜尋商品名稱（可選）</param>
        /// <param name="sortBy">排序：latest / priceAsc / priceDesc / soldCount（預設 latest）</param>
        /// <param name="page">頁碼（預設 1）</param>
        /// <param name="pageSize">每頁筆數（預設 20，上限 50）</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int?     categoryId    = null,
            [FromQuery] int?     subCategoryId = null,
            [FromQuery] int[]?   brandIds      = null,
            [FromQuery] decimal? minPrice      = null,
            [FromQuery] decimal? maxPrice      = null,
            [FromQuery] string?  keyword       = null,
            [FromQuery] string?  sortBy        = null,
            [FromQuery] int      page          = 1,
            [FromQuery] int      pageSize      = 20)
        {
            page     = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var result = await _productService.GetFrontActiveProductsAsync(
                categoryId, keyword, sortBy ?? "latest", page, pageSize,
                subCategoryId, brandIds, minPrice, maxPrice);

            var items = result.Data.Select(p => new ProductListItemDto
            {
                Id            = p.Id,
                Name          = p.Name,
                Price         = p.MinPrice ?? 0m,
                OriginalPrice = null,
                ImageUrl      = p.MainImageUrl ?? string.Empty,
                SoldCount     = p.TotalSales ?? 0,
                Location      = string.Empty,
                CategoryId    = p.CategoryId,
                Rating        = null
            }).ToList();

            return Ok(new
            {
                success = true,
                data = new ProductListResponseDto
                {
                    Items    = items,
                    Total    = result.TotalCount,
                    Page     = result.CurrentPage,
                    PageSize = result.PageSize
                },
                message = ""
            });
        }
    }
}

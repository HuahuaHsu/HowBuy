using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ISpanShop.Models.DTOs.Products;
using ISpanShop.Models.DTOs.Common;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Products;

namespace ISpanShop.Repositories.Products
{
    /// <summary>
    /// 商品 Repository 實作 - 處理商品的 CRUD 操作
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ISpanShopDBContext _context;

        /// <summary>
        /// 建構子 - 注入 DbContext
        /// </summary>
        /// <param name="context">ISpanShop 資料庫上下文</param>
        public ProductRepository(ISpanShopDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// 檢查 SKU 代碼是否已存在
        /// </summary>
        public bool IsSkuExists(string skuCode)
            => _context.ProductVariants.Any(pv => pv.SkuCode == skuCode);

        /// <summary>
        /// 更新商品狀態
        /// </summary>
        public void UpdateProductStatus(int id, byte status)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.Status = status;
                product.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 根據 ID 取得商品詳情（包含圖片與規格）
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <returns>商品實體，若不存在則返回 null</returns>
        public Product? GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 取得所有分類（含父子關聯）
        /// </summary>
        /// <returns>所有分類集合</returns>
        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        /// <summary>
        /// 取得所有商家清單 (Id, StoreName)
        /// </summary>
        /// <returns>商家清單</returns>
        public IEnumerable<(int Id, string Name)> GetStoreOptions()
        {
            return _context.Stores
                .Select(s => new { s.Id, s.StoreName })
                .ToList()
                .Select(s => (s.Id, s.StoreName));
        }
        /// <summary>
        /// 取得所有品牌清單 (Id, Name)
        /// </summary>
        /// <returns>品牌清單</returns>
        public IEnumerable<(int Id, string Name)> GetBrandOptions()
        {
            return _context.Brands
                .Where(b => b.IsDeleted != true)
                .Select(b => new { b.Id, b.Name })
                .ToList()
                .Select(b => (b.Id, b.Name));
        }
        /// <summary>
        /// 根據子分類取得該分類下商品涵蓋的品牌清單
        /// </summary>
        /// <param name="categoryId">子分類 ID；為 null 時回傳全部品牌</param>
        /// <returns>品牌清單（依名稱排序）</returns>
        public IEnumerable<(int Id, string Name)> GetBrandsByCategory(int? categoryId)
        {
            return _context.Products
                .Where(p => categoryId == null || p.CategoryId == categoryId)
                .Where(p => p.BrandId != null && p.Brand != null && p.Brand.IsDeleted != true)
                .Select(p => new { p.Brand!.Id, p.Brand!.Name })
                .Distinct()
                .OrderBy(b => b.Name)
                .ToList()
                .Select(b => (b.Id, b.Name));
        }

        /// <summary>
        /// 批次更新商品上下架狀態
        /// </summary>
        public async Task<int> UpdateBatchStatusAsync(List<int> productIds, byte targetStatus)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                product.Status    = targetStatus;
                product.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return products.Count;
        }

        /// <summary>
        /// 批次更新商品審核狀態
        /// </summary>
        public async Task<int> UpdateBatchReviewStatusAsync(List<int> productIds, int targetReviewStatus, string adminId)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                product.ReviewStatus = targetReviewStatus;
                product.ReviewedBy   = adminId;
                product.ReviewDate   = DateTime.Now;
                product.UpdatedAt    = DateTime.Now;

                // 審核通過 → 狀態改為上架；退回 → 狀態改為審核退回
                if (targetReviewStatus == 1)
                    product.Status = 1; // 上架
                else if (targetReviewStatus == 2)
                    product.Status = 3; // 審核退回
            }

            await _context.SaveChangesAsync();
            return products.Count;
        }

        /// <summary>
        /// 分頁取得商品列表，支援分類篩選與多維度搜尋
        /// </summary>
        public (IEnumerable<Product> Items, int TotalCount) GetProductsPaged(ProductSearchCriteria criteria)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.IsDeleted != true)
                .AsQueryable();

            if (criteria.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);
            else if (criteria.ParentCategoryId.HasValue)
                query = query.Where(p => p.Category.ParentId == criteria.ParentCategoryId.Value);

            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                var kw = criteria.Keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(kw) ||
                    (p.Description != null && p.Description.ToLower().Contains(kw)));
            }

            if (criteria.StoreId.HasValue)
                query = query.Where(p => p.StoreId == criteria.StoreId.Value);

            if (criteria.BrandId.HasValue)
                query = query.Where(p => p.BrandId == criteria.BrandId.Value);

            if (criteria.Status.HasValue)
                query = query.Where(p => p.Status == criteria.Status.Value);
            else
                query = query.Where(p => p.Status != 2 && p.Status != 3);

            if (criteria.StartDate.HasValue)
                query = query.Where(p => p.CreatedAt >= criteria.StartDate.Value);

            if (criteria.EndDate.HasValue)
            {
                var endOfDay = criteria.EndDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.CreatedAt <= endOfDay);
            }

            query = criteria.SortOrder switch
            {
                "name_asc"    => query.OrderBy(p => p.Name),
                "name_desc"   => query.OrderByDescending(p => p.Name),
                "price_asc"   => query.OrderBy(p => p.MinPrice),
                "price_desc"  => query.OrderByDescending(p => p.MinPrice),
                "status_asc"  => query.OrderBy(p => p.Status),
                "status_desc" => query.OrderByDescending(p => p.Status),
                "date_asc"    => query.OrderBy(p => p.CreatedAt),
                "review_desc" => query.OrderByDescending(p => p.ReviewDate),
                _             => query.OrderByDescending(p => p.CreatedAt)
            };

            int totalCount = query.Count();
            var items = query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToList();

            return (items, totalCount);
        }

        public void UpdateProduct(ProductUpdateDto dto)
        {
            var product = _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == dto.Id);
            if (product == null) return;

            product.Name               = dto.Name;
            product.Description        = dto.Description;
            product.CategoryId         = dto.CategoryId;
            product.BrandId            = dto.BrandId;
            product.SpecDefinitionJson = dto.SpecDefinitionJson;
            product.UpdatedAt          = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(dto.MainImageUrl))
            {
                var mainImg = product.ProductImages.FirstOrDefault(i => i.IsMain == true);
                if (mainImg != null)
                    mainImg.ImageUrl = dto.MainImageUrl;
                else
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = dto.MainImageUrl, IsMain = true, SortOrder = 0
                    });
            }
            _context.SaveChanges();
        }

        public void SoftDeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return;
            product.IsDeleted = true;
            product.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }

        public ProductVariant? GetVariantById(int id)
            => _context.ProductVariants.Include(v => v.Product).FirstOrDefault(v => v.Id == id);

        public void AddVariant(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
            _context.SaveChanges();
            RecalcProductPrice(variant.ProductId);
        }

        public void UpdateVariant(ProductVariantUpdateDto dto)
        {
            var variant = _context.ProductVariants.Find(dto.Id);
            if (variant == null) return;
            variant.SkuCode     = dto.SkuCode;
            variant.Price       = dto.Price;
            variant.Stock       = dto.Stock;
            variant.SafetyStock = dto.SafetyStock;
            _context.SaveChanges();
            RecalcProductPrice(variant.ProductId);
        }

        public void SoftDeleteVariant(int id)
        {
            var variant = _context.ProductVariants.Find(id);
            if (variant == null) return;
            variant.IsDeleted = true;
            _context.SaveChanges();
            RecalcProductPrice(variant.ProductId);
        }

        private void RecalcProductPrice(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return;
            var prices = _context.ProductVariants
                .Where(v => v.ProductId == productId && v.IsDeleted != true)
                .Select(v => v.Price)
                .ToList();
            if (prices.Any())
            {
                product.MinPrice = prices.Min();
                product.MaxPrice = prices.Max();
            }
            product.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }

        // ═══════════════════════════════════════════════════════════
        //  非同步實作（async/await + 投影 + 真分頁）
        // ═══════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public async Task<(IEnumerable<ProductListDto> Items, int TotalCount)>
            GetProductsPagedAsync(ProductSearchCriteria criteria)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsDeleted != true)
                .AsQueryable();

            if (criteria.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);
            else if (criteria.ParentCategoryId.HasValue)
                query = query.Where(p => p.Category.ParentId == criteria.ParentCategoryId.Value);

            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                var keyword = criteria.Keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLower().Contains(keyword)));
            }

            if (criteria.StoreId.HasValue)
                query = query.Where(p => p.StoreId == criteria.StoreId.Value);

            if (criteria.BrandId.HasValue)
                query = query.Where(p => p.BrandId == criteria.BrandId.Value);

            if (criteria.Status.HasValue)
                query = query.Where(p => p.Status == criteria.Status.Value);
            else
                // 已退回商品只在審核中心的近期退回紀錄顯示，不出現在商品總覽
                query = query.Where(p => p.Status != 2 && p.Status != 3);

            if (criteria.StartDate.HasValue)
                query = query.Where(p => p.CreatedAt >= criteria.StartDate.Value);

            if (criteria.EndDate.HasValue)
            {
                var endOfDay = criteria.EndDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.CreatedAt <= endOfDay);
            }

            query = criteria.SortOrder switch
            {
                "name_asc"    => query.OrderBy(p => p.Name),
                "name_desc"   => query.OrderByDescending(p => p.Name),
                "price_asc"   => query.OrderBy(p => p.MinPrice),
                "price_desc"  => query.OrderByDescending(p => p.MinPrice),
                "status_asc"  => query.OrderBy(p => p.Status),
                "status_desc" => query.OrderByDescending(p => p.Status),
                "date_asc"    => query.OrderBy(p => p.CreatedAt),
                "review_desc" => query.OrderByDescending(p => p.ReviewDate),
                _             => query.OrderByDescending(p => p.CreatedAt)
            };

            // COUNT 在 SQL 端完成
            int totalCount = await query.CountAsync();

            // Skip/Take 接在 IQueryable 後 → SQL 端分頁；Select 投影只取所需欄位
            var items = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .Select(p => new ProductListDto
                {
                    Id           = p.Id,
                    StoreName    = p.Store != null ? p.Store.StoreName : "未知商店",
                    CategoryName = p.Category != null ? p.Category.Name : "未分類",
                    BrandName    = p.Brand != null ? p.Brand.Name : "未設定",
                    Name         = p.Name,
                    MinPrice     = p.MinPrice,
                    MaxPrice     = p.MaxPrice,
                    Status       = p.Status,
                    CreatedAt    = p.CreatedAt,
                    ReviewStatus = p.ReviewStatus,
                    ReviewedBy   = p.ReviewedBy,
                    ReviewDate   = p.ReviewDate,
                    RejectReason = p.RejectReason,
                    MainImageUrl =
                        p.ProductImages.Where(img => img.IsMain == true)
                                       .Select(img => img.ImageUrl).FirstOrDefault()
                        ?? p.ProductImages.Select(img => img.ImageUrl).FirstOrDefault()
                        ?? "https://via.placeholder.com/400x400?text=No+Image"
                })
                .ToListAsync();

            return (items, totalCount);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductReviewDto>> GetPendingProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.ReviewStatus == 0 && p.IsDeleted != true)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProductReviewDto
                {
                    Id           = p.Id,
                    StoreId      = p.StoreId,
                    CategoryName = p.Category != null ? p.Category.Name : "未分類",
                    BrandName    = p.Brand != null ? p.Brand.Name : "未設定",
                    StoreName    = p.Store != null ? p.Store.StoreName : "未知商店",
                    Name         = p.Name,
                    Description  = p.Description,
                    Status       = p.Status ?? 0,
                    ReviewStatus = p.ReviewStatus,
                    ReviewedBy   = p.ReviewedBy,
                    ReviewDate   = p.ReviewDate,
                    CreatedAt    = p.CreatedAt,
                    UpdatedAt    = p.UpdatedAt,
                    MainImageUrl = p.ProductImages
                        .Where(img => img.IsMain == true)
                        .Select(img => img.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<ProductReviewDto> Items, int TotalCount)>
            GetPendingProductsPagedAsync(int page, int pageSize)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.ReviewStatus == 0 && p.IsDeleted != true)
                .OrderByDescending(p => p.CreatedAt);

            int total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductReviewDto
                {
                    Id           = p.Id,
                    StoreId      = p.StoreId,
                    CategoryName = p.Category != null ? p.Category.Name : "未分類",
                    BrandName    = p.Brand != null ? p.Brand.Name : "未設定",
                    StoreName    = p.Store != null ? p.Store.StoreName : "未知商店",
                    Name         = p.Name,
                    Description  = p.Description,
                    Status       = p.Status ?? 0,
                    ReviewStatus = p.ReviewStatus,
                    ReviewedBy   = p.ReviewedBy,
                    ReviewDate   = p.ReviewDate,
                    CreatedAt    = p.CreatedAt,
                    UpdatedAt    = p.UpdatedAt,
                    MainImageUrl = p.ProductImages
                        .Where(img => img.IsMain == true)
                        .Select(img => img.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();

            return (items, total);
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<ProductReviewDto> Items, int TotalCount)>
            GetRejectedProductsPagedAsync(int page, int pageSize)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.ReviewStatus == 2 && p.IsDeleted != true)
                .OrderByDescending(p => p.ReviewDate ?? p.UpdatedAt);

            int total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductReviewDto
                {
                    Id           = p.Id,
                    StoreId      = p.StoreId,
                    CategoryName = p.Category != null ? p.Category.Name : "未分類",
                    BrandName    = p.Brand != null ? p.Brand.Name : "未設定",
                    StoreName    = p.Store != null ? p.Store.StoreName : "未知商店",
                    Name         = p.Name,
                    Description  = p.Description,
                    Status       = p.Status ?? 0,
                    ReviewStatus = p.ReviewStatus,
                    ReviewedBy   = p.ReviewedBy,
                    ReviewDate   = p.ReviewDate,
                    RejectReason = p.RejectReason,
                    CreatedAt    = p.CreatedAt,
                    UpdatedAt    = p.UpdatedAt,
                    MainImageUrl = p.ProductImages
                        .Where(img => img.IsMain == true)
                        .Select(img => img.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();

            return (items, total);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductReviewDto>> GetRecentRejectedProductsAsync(int top)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.ReviewStatus == 2 && p.IsDeleted != true)
                .OrderByDescending(p => p.UpdatedAt)
                .Take(top)
                .Select(p => new ProductReviewDto
                {
                    Id           = p.Id,
                    StoreId      = p.StoreId,
                    CategoryName = p.Category != null ? p.Category.Name : "未分類",
                    BrandName    = p.Brand != null ? p.Brand.Name : "未設定",
                    StoreName    = p.Store != null ? p.Store.StoreName : "未知商店",
                    Name         = p.Name,
                    Description  = p.Description,
                    Status       = p.Status ?? 0,
                    ReviewStatus = p.ReviewStatus,
                    ReviewedBy   = p.ReviewedBy,
                    ReviewDate   = p.ReviewDate,
                    RejectReason = p.RejectReason,
                    CreatedAt    = p.CreatedAt,
                    UpdatedAt    = p.UpdatedAt,
                    MainImageUrl = p.ProductImages
                        .Where(img => img.IsMain == true)
                        .Select(img => img.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task ApproveProductAsync(int id, string adminId)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            product.Status       = 1;
            product.ReviewStatus = 1;
            product.ReviewedBy   = adminId;
            product.RejectReason = null;
            product.ReviewDate   = DateTime.Now;
            product.UpdatedAt    = DateTime.Now;
            if (product.Name != null && product.Name.StartsWith("[待審核]"))
                product.Name = product.Name.Substring("[待審核]".Length).TrimStart();

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task RejectProductAsync(int id, string adminId, string reason)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            product.Status       = 3;
            product.ReviewStatus = 2;
            product.ReviewedBy   = adminId;
            product.RejectReason = reason;
            product.ReviewDate   = DateTime.Now;
            product.UpdatedAt    = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task SubmitProductForReviewAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            var badWords = new[]
            {
                "高仿", "原單", "槍械", "毒品", "贗品", "假貨", "冒牌", "盜版",
                "走私", "非法", "詐騙", "傳銷", "洗錢", "賭博", "色情",
                "暴力", "恐怖", "炸藥", "大麻", "槍", "彈藥", "仿冒", "武器"
            };
            string combined = (product.Name + " " + (product.Description ?? "")).ToLower();
            string? hit = badWords.FirstOrDefault(w => combined.Contains(w.ToLower()));

            if (hit != null)
            {
                product.Status       = 3;
                product.ReviewStatus = 2;
                product.ReviewedBy   = "System";
                product.RejectReason = $"自動攔截：內容含違禁詞「{hit}」";
                product.ReviewDate   = DateTime.Now;
            }
            else
            {
                product.Status       = 2;
                product.ReviewStatus = 0;
                product.ReviewedBy   = null;
                product.RejectReason = null;
                product.ReviewDate   = null;
            }
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CleanupExpiredRejectedAsync(int expirationSeconds)
        {
            var cutoff = DateTime.Now.AddSeconds(-expirationSeconds);
            var expired = await _context.Products
                .Where(p => p.ReviewStatus == 2
                         && p.ReviewDate != null
                         && p.ReviewDate <= cutoff
                         && p.IsDeleted != true)
                .ToListAsync();

            foreach (var p in expired)
            {
                p.IsDeleted = true;
                p.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return expired.Count;
        }

        /// <inheritdoc/>
        public async Task ResetToPendingAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            product.Status       = 2;
            product.ReviewStatus = 0;
            product.ReviewedBy   = null;
            product.ReviewDate   = null;
            product.RejectReason = null;
            product.UpdatedAt    = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<(int Total, int Published, int Unpublished, int Pending)> GetStatusCountsAsync()
        {
            var q = _context.Products.Where(p => p.IsDeleted != true);
            return (
                await q.CountAsync(),
                await q.CountAsync(p => p.Status == 1),
                await q.CountAsync(p => p.Status == 0),
                await q.CountAsync(p => p.ReviewStatus == 0)
            );
        }

        /// <inheritdoc/>
        public async Task ForceUnpublishAsync(int id, string? reason)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            product.Status       = 0;
            product.RejectReason = reason;
            product.UpdatedAt    = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<SimulateAutoReviewResult> SimulateAutoReviewAsync()
        {
            // ── 第一階段：複製現有商品，新增 15 筆測試商品 ──────────────
            var templates = await _context.Products
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.Id)
                .Take(3)
                .ToListAsync();

            if (templates.Count < 3)
                throw new InvalidOperationException("資料庫中現有商品不足 3 筆，無法執行模擬。");

            var newProducts = new List<Product>();

            // 前 5 筆：黑名單（高仿、盜版）
            for (int i = 0; i < 5; i++)
            {
                var template = templates[i % 3];
                var prefix = i < 3 ? "[高仿] " : "[盜版] ";
                newProducts.Add(new Product
                {
                    StoreId            = template.StoreId,
                    CategoryId         = template.CategoryId,
                    BrandId            = template.BrandId,
                    Name               = prefix + template.Name,
                    Description        = template.Description,
                    VideoUrl           = template.VideoUrl,
                    SpecDefinitionJson = template.SpecDefinitionJson,
                    MinPrice           = template.MinPrice,
                    MaxPrice           = template.MaxPrice,
                    Status             = 2,
                    ReviewStatus       = 0,
                    ReviewedBy         = null,
                    RejectReason       = null,
                    ReviewDate         = null,
                    CreatedAt          = DateTime.Now,
                    UpdatedAt          = DateTime.Now,
                    IsDeleted          = false
                });
            }

            // 中 5 筆：灰名單（客製化、二手）
            for (int i = 0; i < 5; i++)
            {
                var template = templates[i % 3];
                var prefix = i < 3 ? "[客製化] " : "[二手] ";
                newProducts.Add(new Product
                {
                    StoreId            = template.StoreId,
                    CategoryId         = template.CategoryId,
                    BrandId            = template.BrandId,
                    Name               = prefix + template.Name,
                    Description        = template.Description,
                    VideoUrl           = template.VideoUrl,
                    SpecDefinitionJson = template.SpecDefinitionJson,
                    MinPrice           = template.MinPrice,
                    MaxPrice           = template.MaxPrice,
                    Status             = 2,
                    ReviewStatus       = 0,
                    ReviewedBy         = null,
                    RejectReason       = null,
                    ReviewDate         = null,
                    CreatedAt          = DateTime.Now,
                    UpdatedAt          = DateTime.Now,
                    IsDeleted          = false
                });
            }

            // 後 5 筆：正常商品（無前綴）
            for (int i = 0; i < 5; i++)
            {
                var template = templates[i % 3];
                newProducts.Add(new Product
                {
                    StoreId            = template.StoreId,
                    CategoryId         = template.CategoryId,
                    BrandId            = template.BrandId,
                    Name               = template.Name + $" (測試{i + 1})",
                    Description        = template.Description,
                    VideoUrl           = template.VideoUrl,
                    SpecDefinitionJson = template.SpecDefinitionJson,
                    MinPrice           = template.MinPrice,
                    MaxPrice           = template.MaxPrice,
                    Status             = 2,
                    ReviewStatus       = 0,
                    ReviewedBy         = null,
                    RejectReason       = null,
                    ReviewDate         = null,
                    CreatedAt          = DateTime.Now,
                    UpdatedAt          = DateTime.Now,
                    IsDeleted          = false
                });
            }

            _context.Products.AddRange(newProducts);
            await _context.SaveChangesAsync();

            // ── 複製圖片：為每個新商品複製樣板商品的圖片 ──────────────
            for (int i = 0; i < newProducts.Count; i++)
            {
                var newProduct = newProducts[i];
                var template = templates[i % 3];

                var sourceImages = await _context.ProductImages
                    .Where(img => img.ProductId == template.Id)
                    .ToListAsync();

                foreach (var img in sourceImages)
                {
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId   = newProduct.Id,
                        VariantId   = null,
                        ImageUrl    = img.ImageUrl,
                        IsMain      = img.IsMain,
                        SortOrder   = img.SortOrder
                    });
                }
            }
            await _context.SaveChangesAsync();

            // ── 第二階段：對新增的 15 筆商品執行自動審核邏輯 ──────────────
            string[] bannedWords     = { "高仿", "盜版" };
            string[] suspiciousWords = { "客製化", "二手" };

            int approvedCount     = 0;
            int rejectedCount     = 0;
            int manualReviewCount = 0;

            foreach (var p in newProducts)
            {
                // 情境 A：觸發黑名單 → 直接退回
                var banned = bannedWords.FirstOrDefault(w =>
                    (p.Name        != null && p.Name.Contains(w)) ||
                    (p.Description != null && p.Description.Contains(w)));

                if (banned != null)
                {
                    p.ReviewStatus = 2;
                    p.RejectReason = $"系統攔截：包含違禁詞 [{banned}]";
                    p.ReviewedBy   = "系統自動判讀";
                    p.ReviewDate   = DateTime.Now;
                    p.Status       = 3;
                    rejectedCount++;
                    continue;
                }

                // 情境 B：觸發灰名單 → 維持待審核，寫入人工複審提示
                var suspicious = suspiciousWords.FirstOrDefault(w =>
                    (p.Name        != null && p.Name.Contains(w)) ||
                    (p.Description != null && p.Description.Contains(w)));

                if (suspicious != null)
                {
                    p.RejectReason = $"系統標示：疑似敏感詞 [{suspicious}]，需人工複審";
                    p.ReviewedBy   = null;
                    manualReviewCount++;
                    continue;
                }

                // 情境 C：皆無觸發 → 自動通過
                p.ReviewStatus = 1;
                p.ReviewedBy   = "系統自動判讀";
                p.ReviewDate   = DateTime.Now;
                p.Status       = 1;
                approvedCount++;
            }

            await _context.SaveChangesAsync();

            return new SimulateAutoReviewResult
            {
                ApprovedCount     = approvedCount,
                RejectedCount     = rejectedCount,
                ManualReviewCount = manualReviewCount
            };
        }
    }
}

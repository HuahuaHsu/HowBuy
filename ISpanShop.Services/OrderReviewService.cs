using ISpanShop.Models.DTOs;
using ISpanShop.Repositories.Orders;
using ISpanShop.Services.ContentModeration; // 加入命名空間
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISpanShop.Services
{
    public class OrderReviewService : IOrderReviewService
    {
        private readonly IOrderReviewRepository _repo;
        private readonly ISensitiveWordService _sensitiveWordService; // 注入工具
        private readonly ISpanShop.Models.EfModels.ISpanShopDBContext _context;

        public OrderReviewService(IOrderReviewRepository repo, ISensitiveWordService sensitiveWordService, ISpanShop.Models.EfModels.ISpanShopDBContext context)
        {
            _repo = repo;
            _sensitiveWordService = sensitiveWordService;
            _context = context;
        }

        public async Task<List<OrderReviewDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            var dtos = new List<OrderReviewDto>();

            foreach (var e in entities)
            {
                // 自動偵測內容是否包含違禁詞
                bool isAutoFlagged = await _sensitiveWordService.HasSensitiveWordAsync(e.Comment);

                dtos.Add(new OrderReviewDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    OrderId = e.OrderId,
                    Rating = e.Rating,
                    Comment = e.Comment,
                    StoreReply = e.StoreReply,
                    IsHidden = e.IsHidden ?? false, 
                    IsAutoFlagged = isAutoFlagged, // 賦予系統掃描後的狀態
                    CreatedAt = e.CreatedAt ?? DateTime.MinValue,
                    ImageUrls = e.ReviewImages.Select(img => img.ImageUrl).ToList(),
                    ProductMainImage = e.Order?.OrderDetails?.FirstOrDefault()?.Product?.ProductImages?
                                        .FirstOrDefault(pi => pi.IsMain == true)?.ImageUrl 
                                        ?? e.Order?.OrderDetails?.FirstOrDefault()?.Product?.ProductImages?.FirstOrDefault()?.ImageUrl
                                        ?? "/images/no-image.png"
                });
            }

            return dtos.OrderByDescending(x => x.CreatedAt).ToList();
        }

        // [新增] 前台新增評論 (包含自動審查邏輯)
        public async Task AddReviewAsync(OrderReviewDto dto)
        {
            // 自動偵測內容是否包含違禁詞
            bool hasSensitiveWord = await _sensitiveWordService.HasSensitiveWordAsync(dto.Comment);

            var entity = new ISpanShop.Models.EfModels.OrderReview
            {
                UserId = dto.UserId,
                OrderId = dto.OrderId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                // 如果有敏感字，寫入當下就直接被「隱藏」
                IsHidden = hasSensitiveWord, 
                CreatedAt = dto.CreatedAt,
                ReviewImages = dto.ImageUrls.Select(url => new ISpanShop.Models.EfModels.ReviewImage
                {
                    ImageUrl = url
                }).ToList()
            };

            await _repo.CreateAsync(entity);
        }

        public async Task ToggleHiddenStatusAsync(int id)
        {
            var review = await _repo.GetByIdAsync(id);
            if (review != null)
            {
                // 核心邏輯：狀態反轉 (如果被隱藏就解開，如果沒隱藏就隱藏)
                bool currentStatus = review.IsHidden ?? false;
                review.IsHidden = !currentStatus;

                await _repo.UpdateAsync(review);
            }
        }

        public async Task GenerateMockReviewsAsync(int productId, int count)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            var users = _context.Users.Where(u => u.RoleId == 1).Take(10).ToList();
            if (!users.Any()) return;

            var comments = new[] { 
                "品質真的很棒，值得推薦！", 
                "發貨速度很快，包裝也很細心。", 
                "這款商品真的很好用，cp值很高。", 
                "雖然有一點色差，但整體來說還不錯。", 
                "物超所值，下次還會再買。",
                "外觀精美，手感也很好。",
                "實品跟照片一樣，沒有落差。",
                "包裝完整，商品沒有損傷。",
                "穿起來很舒服，大小剛好。",
                "送貨人員態度很好，商品也很滿意。"
            };

            var random = new Random();
            
            for (int i = 0; i < count; i++)
            {
                var user = users[random.Next(users.Count)];
                
                var order = new ISpanShop.Models.EfModels.Order
                {
                    UserId = user.Id,
                    StoreId = product.StoreId,
                    OrderNumber = "MOCK" + DateTime.Now.ToString("yyyyMMddHHmmss") + i + random.Next(100, 999),
                    Status = 4, // 已完成
                    TotalAmount = product.MinPrice ?? 0,
                    FinalAmount = product.MinPrice ?? 0,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 30)),
                    CompletedAt = DateTime.Now.AddDays(-random.Next(0, 1)),
                    RecipientName = user.Account,
                    RecipientPhone = "0912345678",
                    RecipientAddress = "模擬地址"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var detail = new ISpanShop.Models.EfModels.OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.MinPrice,
                    Quantity = 1,
                    VariantName = "預設規格"
                };
                _context.OrderDetails.Add(detail);
                await _context.SaveChangesAsync();

                var review = new ISpanShop.Models.EfModels.OrderReview
                {
                    OrderId = order.Id,
                    UserId = user.Id,
                    Rating = (byte)random.Next(4, 6),
                    Comment = comments[random.Next(comments.Length)],
                    IsHidden = false,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(0, 5))
                };
                _context.OrderReviews.Add(review);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ISpanShop.Models.DTOs.Products.FrontProductReviewVm>> GetReviewsByProductIdAsync(int productId)
        {
            var entities = await _repo.GetAllAsync();

            var reviews = entities
                .Where(r => r.IsHidden != true && r.Order != null && r.Order.OrderDetails.Any(od => od.ProductId == productId))
                .Select(r => new ISpanShop.Models.DTOs.Products.FrontProductReviewVm
                {
                    Id = r.Id,
                    UserName = MaskUserName(r.User?.Account ?? "Guest"),
                    UserAvatar = r.User?.MemberProfile?.AvatarUrl ?? "/images/default-avatar.png",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    StoreReply = r.StoreReply,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                    VariantName = r.Order.OrderDetails.FirstOrDefault(od => od.ProductId == productId)?.VariantName,
                    ImageUrls = r.ReviewImages.Select(ri => ri.ImageUrl).ToList()
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return reviews;
        }

        private string MaskUserName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "****";
            if (name.Length <= 2) return name[0] + "****";
            return name[0] + "****" + name[name.Length - 1];
        }
    }
}
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ISpanShop.Common.Enums;
using ISpanShop.Models.EfModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.MVC.Controllers.Api
{
    [ApiController]
    [Route("api/front/demo")]
    [Authorize(AuthenticationSchemes = "FrontendJwt")]
    public class DemoOrderController : ControllerBase
    {
        private readonly ISpanShopDBContext _context;

        public DemoOrderController(ISpanShopDBContext context)
        {
            _context = context;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateDemoOrder([FromBody] CreateDemoOrderRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            // 驗證 demo 商品 ID (只允許 Apple AirPods 4499)
            if (request.ProductId != 500)
            {
                return BadRequest(new { message = "無效的 Demo 商品" });
            }

            try
            {
                // 建立訂單
                var order = new Order
                {
                    OrderNumber = $"DEMO{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}",
                    UserId = userId,
                    StoreId = 1,
                    TotalAmount = 4499,
                    ShippingFee = 0,
                    DiscountAmount = 0,
                    FinalAmount = 4499,
                    Status = (byte)OrderStatus.Completed,  // 直接標記為已完成
                    PaymentDate = DateTime.Now,
                    CompletedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    RecipientName = request.RecipientName ?? "示範收貨人",
                    RecipientPhone = request.RecipientPhone ?? "0900000000",
                    RecipientAddress = request.RecipientAddress ?? "示範地址",
                    Note = "[Demo 訂單] 用於會員等級展示"
                };

                // 新增訂單明細
                var orderDetail = new OrderDetail
                {
                    ProductId = 500,
                    ProductName = "Apple AirPods 藍牙耳機 - 聯名精裝版",
                    VariantName = "標準版本",
                    SkuCode = "SKU-AIRPODS-500",
                    CoverImage = null,
                    Price = 4499,
                    Quantity = request.Quantity
                };

                order.OrderDetails.Add(orderDetail);

                // 保存到資料庫
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "✅ Demo 訂單已成功建立！已標記為已完成狀態，立即生效會員級別計算。",
                    orderId = order.Id,
                    orderNumber = order.OrderNumber,
                    totalAmount = order.TotalAmount,
                    status = "已完成",
                    tip = "刷新頁面後會看到會員級別與消費金額更新"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo 訂單建立錯誤: {ex.Message}\n{ex.StackTrace}");
                return BadRequest(new { message = $"建立訂單時發生錯誤：{ex.Message}" });
            }
        }
    }

    public class CreateDemoOrderRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using ISpanShop.Repositories.Orders;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ISpanShop.MVC.Controllers.Api.Orders
{
	[Route("api/orders")]
	[ApiController]
	public class OrdersApiController : ControllerBase
	{
		private readonly IOrderRepository _orderRepo;

		public OrdersApiController(IOrderRepository orderRepo)
		{
			_orderRepo = orderRepo;
		}

		// GET: api/orders/1
		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderDetail(long id)
		{
			// 呼叫 Repository 取得訂單，包含相關連動實體
			var order = await _orderRepo.GetOrderByIdAsync(id);

			if (order == null)
			{
				return NotFound(new { message = "找不到該訂單" });
			}

			// 這邊使用了您資料庫生成的正確屬性名稱：RecipientName, Price, VariantName, CoverImage
			var result = new
			{
				id = order.Id.ToString(),
				orderNumber = order.OrderNumber,
				receiverName = order.RecipientName, // 已修正：原為 ReceiverName
				receiverPhone = order.RecipientPhone, // 已修正：原為 ReceiverPhone
				receiverAddress = order.RecipientAddress, // 已修正：原為 ReceiverAddress
				finalAmount = order.FinalAmount,
				status = order.Status,
				createdAt = order.CreatedAt,
				items = order.OrderDetails.Select(d => new {
					id = d.Id,
					productId = d.ProductId,
					productName = d.ProductName ?? "未知商品",
					variantName = d.VariantName ?? "預設規格",
					imageUrl = d.CoverImage ?? "/images/no-image.png",
					unitPrice = d.Price ?? 0, // 已修正：原為 UnitPrice
					quantity = d.Quantity,
					subTotal = (d.Price ?? 0) * d.Quantity
				}).ToList()
			};

			return Ok(result);
		}
	}
}
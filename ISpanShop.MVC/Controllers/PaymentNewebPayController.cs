using ISpanShop.Models.EfModels;
using ISpanShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace ISpanShop.MVC.Controllers
{
	public class PaymentNewebPayController : Controller
	{
		private readonly ISpanShopDBContext _context;
		private readonly NewebPayService _newebPayService;

		public PaymentNewebPayController(ISpanShopDBContext context, NewebPayService newebPayService)
		{
			_context = context;
			_newebPayService = newebPayService;
		}

		[HttpGet]
		public async Task<IActionResult> Pay(string orderNumber)
		{
			var order = await _context.Orders
				.Include(o => o.OrderDetails)
				.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

			if (order == null)
				return NotFound("訂單不存在");

			string merchantTradeNo = _newebPayService.GenerateMerchantTradeNo(order);
			var parameters = _newebPayService.GetNewebPayParameters(order, merchantTradeNo);

			// 生成自動送出表單
			var sb = new StringBuilder();
			sb.AppendLine("<h4>正在轉向藍新支付...</h4>");
			sb.AppendLine("<form id='payForm' " +
						  "action='https://ccore.newebpay.com/MPG/mpg_gateway' " +
						  "method='POST'>");

			foreach (var p in parameters)
			{
				sb.AppendLine($"<input type='hidden' name='{p.Key}' value='{p.Value}' />");
			}

			sb.AppendLine("</form>");
			sb.AppendLine("<script>document.getElementById('payForm').submit();</script>");

			return Content(sb.ToString(), "text/html");
		}

		[HttpPost]
		public IActionResult Return()
		{
			// 交易回傳模擬成功
			ViewBag.Message = "交易成功（模擬）";
			ViewBag.MerchantTradeNo = Request.Form["MerchantTradeNo"];
			ViewBag.Amount = Request.Form["Amt"];
			ViewBag.PaymentType = Request.Form["ChoosePayment"];
			return View();
		}
	}
}
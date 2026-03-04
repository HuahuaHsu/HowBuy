using ISpanShop.Models.EfModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISpanShop.Services
{
	public class PaymentService
	{
		private readonly ISpanShopDBContext _context;

		public PaymentService(ISpanShopDBContext context)
		{
			_context = context;
		}

		/// <summary>
		/// 產生綠界專用的交易單號 (限制 20 字)
		/// 格式：TS + 月日時分秒 + 4位隨機數 (例如: TS03251430058812)
		/// </summary>
		public string GenerateMerchantTradeNo()
		{
			var timestamp = DateTime.Now.ToString("MMddHHmmss");
			var random = new Random().Next(1000, 9999).ToString();
			return $"TS{timestamp}{random}";
		}

		/// <summary>
		/// 處理綠界付款成功後的回傳紀錄
		/// </summary>
		public async Task<bool> ProcessPaymentCallbackAsync(PaymentLog log, long orderId)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// 1. 寫入付款紀錄 (使用你 DbContext 中的 PaymentLogs)
					_context.PaymentLogs.Add(log);

					// 2. 更新訂單狀態為「已付款」(假設 Status 1 是已付款)
					var order = await _context.Orders.FindAsync(orderId);
					if (order != null)
					{
						order.Status = 1;
						order.PaymentDate = DateTime.Now;
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return true;
				}
				catch
				{
					await transaction.RollbackAsync();
					return false;
				}
			}
		}
	}
}

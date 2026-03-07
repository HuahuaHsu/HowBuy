using ISpanShop.Models.EfModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ISpanShop.Services
{
	public class NewebPayService
	{
		// 藍新測試帳號（官方提供）
		private const string MerchantID = "S123456789";
		private const string HashKey = "1234567890123456";
		private const string HashIV = "1234567890123456";

		public string GenerateMerchantTradeNo(Order order)
		{
			// 產生唯一交易編號
			return $"N{order.Id:D6}{DateTime.Now:HHmmss}";
		}

		public Dictionary<string, string> GetNewebPayParameters(Order order, string merchantTradeNo)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "MerchantID", MerchantID },
				{ "MerchantTradeNo", merchantTradeNo },
				{ "Amt", ((int)order.FinalAmount).ToString() },
				{ "ItemDesc", "課程影片" },
				{ "TradeLimit", "900" },
				{ "ReturnURL", "https://localhost:7230/PaymentNewebPay/Return" },
				{ "ChoosePayment", "ALL" },
				{ "EncryptType", "1" }
			};

			string checkValue = GenerateCheckValue(parameters);
			parameters.Add("CheckValue", checkValue);

			return parameters;
		}

		private string GenerateCheckValue(Dictionary<string, string> parameters)
		{
			var sortedParams = new SortedDictionary<string, string>(parameters);
			var sb = new StringBuilder();
			sb.Append($"HashKey={HashKey}");
			foreach (var kv in sortedParams)
			{
				sb.Append($"&{kv.Key}={kv.Value}");
			}
			sb.Append($"&HashIV={HashIV}");

			string raw = sb.ToString();

			using (var sha256 = SHA256.Create())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(raw);
				byte[] hash = sha256.ComputeHash(bytes);
				return BitConverter.ToString(hash).Replace("-", "").ToUpper();
			}
		}
	}
}
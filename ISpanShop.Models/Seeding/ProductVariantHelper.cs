using System;
using System.Collections.Generic;
using System.Linq;
using ISpanShop.Models.EfModels;

namespace ISpanShop.Models.Seeding
{
	/// <summary>
	/// 商品規格變體產生器 (從 DataSeeder 抽離，可獨立重複使用)
	/// ★ 未來後台「新增商品」功能如需自動產生規格，也可直接呼叫此 Helper
	/// </summary>
	public static class ProductVariantHelper
	{
		private static readonly Random _random = new Random();

		/// <summary>
		/// 笛卡兒積的組合結果
		/// </summary>
		public class Combination
		{
			public List<string> Values { get; set; } = new();
		}

		/// <summary>
		/// ★ 核心方法：根據分類產生所有規格組合的 ProductVariant 清單
		/// 例如：手機 (容量 x 顏色) = 4x3 = 12 種組合，最多取 maxCombinations 組
		/// </summary>
		public static List<ProductVariant> GenerateVariants(string apiCategory, int basePriceTwd, int maxCombinations = 6)
		{
			var variants = new List<ProductVariant>();

			// 沒有定義規格的分類 → 只給一個「標準版」
			if (!SeederMappings.CategorySpecMap.ContainsKey(apiCategory))
			{
				variants.Add(new ProductVariant
				{
					SkuCode = GenerateSkuCode(),
					VariantName = "標準版",
					SpecValueJson = System.Text.Json.JsonSerializer.Serialize(
						new Dictionary<string, string> { { "規格", "標準版" } }),
					Price = basePriceTwd,
					Stock = _random.Next(50, 201),
					SafetyStock = 10,
					IsDeleted = false
				});
				return variants;
			}

			var dimensions = SeederMappings.CategorySpecMap[apiCategory];

			// 用遞迴產生笛卡兒積 (所有組合)，隨機取前 maxCombinations 個
			var selected = CartesianProduct(dimensions)
				.OrderBy(_ => _random.Next())
				.Take(maxCombinations)
				.ToList();

			for (int i = 0; i < selected.Count; i++)
			{
				var combo = selected[i];

				// 組合名稱，例如「128GB / 午夜黑」
				var variantName = string.Join(" / ", combo.Values);

				// 規格 JSON，例如 {"容量":"128GB","顏色":"午夜黑"}
				var specDict = new Dictionary<string, string>();
				for (int d = 0; d < dimensions.Count; d++)
				{
					specDict[dimensions[d].Name] = combo.Values[d];
				}
				var specJson = System.Text.Json.JsonSerializer.Serialize(specDict);

				// 價格浮動：根據組合索引加一些差價 (模擬真實情況，容量越大越貴)
				int priceOffset = i * _random.Next(50, 300);

				variants.Add(new ProductVariant
				{
					SkuCode = GenerateSkuCode(),
					VariantName = variantName,
					SpecValueJson = specJson,
					Price = basePriceTwd + priceOffset,
					Stock = _random.Next(20, 300),
					SafetyStock = _random.Next(5, 20),
					IsDeleted = false
				});
			}

			return variants;
		}

		/// <summary>
		/// 笛卡兒積：將多個維度的所有值做排列組合
		/// 例如：[["S","M"],["黑","白"]] → [("S","黑"),("S","白"),("M","黑"),("M","白")]
		/// </summary>
		public static List<Combination> CartesianProduct(List<SeederMappings.SpecDimension> dimensions)
		{
			var result = new List<Combination> { new Combination() };

			foreach (var dim in dimensions)
			{
				var newResult = new List<Combination>();
				foreach (var existing in result)
				{
					foreach (var val in dim.Values)
					{
						var newCombo = new Combination
						{
							Values = new List<string>(existing.Values) { val }
						};
						newResult.Add(newCombo);
					}
				}
				result = newResult;
			}

			return result;
		}

		/// <summary>產生 8 碼大寫 SKU 代碼</summary>
		private static string GenerateSkuCode()
			=> Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
	}
}

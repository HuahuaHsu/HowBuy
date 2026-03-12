namespace ISpanShop.Models.DTOs.Products
{
    /// <summary>
    /// 商品規格變體新增用 DTO - 用於接收規格變體資訊
    /// </summary>
    public class ProductVariantCreateDto
    {
        public string? SkuCode { get; set; }
        public required string VariantName { get; set; }
        /// <summary>規格值 JSON - 儲存規格屬性和對應值</summary>
        public required string SpecValueJson { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SafetyStock { get; set; }
    }
}

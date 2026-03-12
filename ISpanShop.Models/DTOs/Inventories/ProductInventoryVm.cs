using System.Collections.Generic;

namespace ISpanShop.Models.DTOs.Inventories
{
    /// <summary>單一 SKU 規格的庫存資訊（子層）</summary>
    public class SkuInventoryVm
    {
        public int    VariantId   { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string SkuCode     { get; set; } = string.Empty;
        public int    Stock       { get; set; }
        public int    SafetyStock { get; set; }

        public bool IsZeroStock => Stock == 0;
        public bool IsLowStock  => Stock > 0 && Stock <= SafetyStock;
    }

    /// <summary>以商品為群組的庫存資訊（父層），包含其所有 SKU 子層</summary>
    public class ProductInventoryVm
    {
        public int    ProductId     { get; set; }
        public string ProductName   { get; set; } = string.Empty;
        public string StoreName     { get; set; } = string.Empty;
        public string CategoryName  { get; set; } = string.Empty;

        /// <summary>底下所有 SKU 庫存加總</summary>
        public int TotalStock { get; set; }

        /// <summary>底下 SKU 的規格數量</summary>
        public int SkuCount { get; set; }

        /// <summary>整體狀態："zero" | "low" | "normal"</summary>
        public string OverallStatus { get; set; } = "normal";

        public List<SkuInventoryVm> Skus { get; set; } = new();

        // ── 便利屬性 ──────────────────────────────────────
        public bool IsZeroStatus   => OverallStatus == "zero";
        public bool IsLowStatus    => OverallStatus == "low";
        public bool IsNormalStatus => OverallStatus == "normal";
    }
}

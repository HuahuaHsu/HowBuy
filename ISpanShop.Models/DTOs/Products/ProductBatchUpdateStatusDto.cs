using System.Collections.Generic;

namespace ISpanShop.Models.DTOs.Products
{
    /// <summary>
    /// 批次更新商品狀態 DTO
    /// </summary>
    public class ProductBatchUpdateStatusDto
    {
        public List<int> ProductIds { get; set; } = new List<int>();
        /// <summary>目標狀態：1 為上架，0 為下架</summary>
        public byte TargetStatus { get; set; }
    }
}

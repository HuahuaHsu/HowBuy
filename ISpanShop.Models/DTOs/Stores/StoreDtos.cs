using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISpanShop.Models.DTOs.Stores
{
    public class StoreDto
    {
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public string OwnerAccount { get; set; } // Users.Account（店主帳號）
        public string StoreName { get; set; } // Stores.StoreName
        public string? Description { get; set; } // Stores.Description
        public bool IsVerified { get; set; } // Stores.IsVerified
        public bool IsBlacklisted { get; set; } // Users.IsBlacklisted
        public DateTime? CreatedAt { get; set; } // Stores.CreatedAt
        public int ProductCount { get; set; } // 商品數量
    }

    public class StoreDetailDto
    {
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public string OwnerAccount { get; set; }
        public string StoreName { get; set; }
        public string? Description { get; set; }
        public bool IsVerified { get; set; }
        public bool IsBlacklisted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int ProductCount { get; set; } // 商品總數（IsDeleted = 0）
        public int ActiveProductCount { get; set; } // 上架中商品數（Status = 1）
        public int TotalSales { get; set; } // 所有商品 TotalSales 加總
    }
}

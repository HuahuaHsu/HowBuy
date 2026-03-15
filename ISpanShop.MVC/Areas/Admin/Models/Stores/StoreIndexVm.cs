using ISpanShop.Models.DTOs.Stores;
using System.Collections.Generic;

namespace ISpanShop.MVC.Areas.Admin.Models.Stores
{
    public class StoreIndexVm
    {
        public List<StoreDto> Stores { get; set; } = new();
        public string? Message { get; set; }
    }
}

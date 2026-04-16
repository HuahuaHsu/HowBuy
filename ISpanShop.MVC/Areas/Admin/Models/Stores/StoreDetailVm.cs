using ISpanShop.Models.DTOs.Stores;

namespace ISpanShop.MVC.Areas.Admin.Models.Stores
{
    public class StoreDetailVm
    {
        public StoreDetailDto? Store { get; set; }
        public string? Message { get; set; }
    }
}

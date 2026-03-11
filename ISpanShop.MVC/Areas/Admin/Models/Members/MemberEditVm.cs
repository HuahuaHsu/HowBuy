using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISpanShop.MVC.Areas.Admin.Models.Members
{
    public class MemberEditVm
    {
        public int UserId { get; set; }
        public string Account { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public string LevelName { get; set; }
        public int PointBalance { get; set; }
        
        public string City { get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        
        public bool IsBlacklisted { get; set; }
        public bool IsSeller { get; set; }

        // 圖片上傳
        public IFormFile? AvatarFile { get; set; }

        public List<SelectListItem> CityOptions { get; set; } = new();
        public List<SelectListItem> RegionOptions { get; set; } = new();
    }
}

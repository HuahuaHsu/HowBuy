using ISpanShop.Models.DTOs.Admins;
using System.Collections.Generic;

namespace ISpanShop.MVC.Areas.Admin.Models.Admins
{
    public class AdminEditVm
    {
        public int UserId { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public int? AdminLevelId { get; set; }
        public bool IsBlacklisted { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsSelf { get; set; }

        public List<AdminLevelDto> AdminLevelOptions { get; set; } = new List<AdminLevelDto>();
    }
}
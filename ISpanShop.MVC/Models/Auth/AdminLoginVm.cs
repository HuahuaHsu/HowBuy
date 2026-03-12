using System.ComponentModel.DataAnnotations;

namespace ISpanShop.MVC.Models.Auth
{
    public class AdminLoginVm
    {
        [Required(ErrorMessage = "請輸入帳號")]
        [Display(Name = "帳號")]
        public string Account { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        public string? Message { get; set; }
    }
}

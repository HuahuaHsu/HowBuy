using System.ComponentModel.DataAnnotations;

namespace ISpanShop.MVC.Models.Auth
{
    public class AdminChangePasswordVm
    {
        [Required(ErrorMessage = "請輸入新密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        [MinLength(8, ErrorMessage = "密碼長度至少需 8 個字元")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "密碼必須包含大小寫英文字母與數字")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "請確認新密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "兩次密碼輸入不一致")]
        public string ConfirmPassword { get; set; }

        public string? Message { get; set; }
    }
}

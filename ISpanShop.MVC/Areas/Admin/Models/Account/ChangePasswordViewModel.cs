using System.ComponentModel.DataAnnotations;

namespace ISpanShop.MVC.Areas.Admin.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "請輸入新密碼")]
        [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "密碼必須包含大小寫英文字母與數字")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }
}

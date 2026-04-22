using System;
using System.ComponentModel.DataAnnotations;

namespace ISpanShop.Models.DTOs.Promotions
{
    public class CreatePromotionDto
    {
        [Required(ErrorMessage = "活動名稱為必填")]
        [StringLength(100, ErrorMessage = "活動名稱最多 100 字")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "活動描述最多 500 字")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "活動類型為必填")]
        [Range(1, 3, ErrorMessage = "活動類型必須為 1-3")]
        public int PromotionType { get; set; }

        [Required(ErrorMessage = "開始時間為必填")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "結束時間為必填")]
        public DateTime EndTime { get; set; }
    }
}

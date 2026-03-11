using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISpanShop.Models.EfModels
{
    public class SensitiveWordCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // 導覽屬性：一個分類可以包含多個敏感字
        public virtual ICollection<SensitiveWord> SensitiveWords { get; set; }
    }
}
using System.Collections.Generic;

namespace ISpanShop.Models.DTOs.Categories
{
    /// <summary>
    /// 分類規格 DTO - 用於資料傳輸與顯示
    /// </summary>
    public class CategorySpecDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        /// <summary>text=文字輸入, select=下拉選單, checkbox=多選框, radio=單選框</summary>
        public string InputType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        /// <summary>允許賣家在預設選項清單外自行輸入選項（僅適用於 select/checkbox/radio 類型）</summary>
        public bool AllowCustomInput { get; set; }
        /// <summary>已綁定的分類名稱列表</summary>
        public List<string> BoundCategoryNames { get; set; } = new List<string>();
    }
}

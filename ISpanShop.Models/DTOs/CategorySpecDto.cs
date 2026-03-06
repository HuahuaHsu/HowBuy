using System.Collections.Generic;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 分類規格 DTO - 用於資料傳輸與顯示
    /// </summary>
    public class CategorySpecDto
    {
        /// <summary>
        /// 規格 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 規格名稱 (例如：顏色、尺寸、容量)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 輸入方式 (text=文字輸入, select=下拉選單, checkbox=多選框, radio=單選框)
        /// </summary>
        public string InputType { get; set; } = string.Empty;

        /// <summary>
        /// 是否為必填項目
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 排序順序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 選項名稱列表 (僅當 InputType 為 select/checkbox/radio 時有值)
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// 允許賣家在預設選項清單外自行輸入選項（僅適用於 select/checkbox/radio 類型）
        /// </summary>
        public bool AllowCustomInput { get; set; }

        /// <summary>已綁定的分類名稱列表</summary>
        public List<string> BoundCategoryNames { get; set; } = new List<string>();
    }
}

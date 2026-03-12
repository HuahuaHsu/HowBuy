namespace ISpanShop.Models.DTOs.Categories
{
    /// <summary>
    /// 分類 DTO - 用於前端下拉選單
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        /// <summary>父分類 ID（null 表示為主分類）</summary>
        public int? ParentId { get; set; }
    }
}

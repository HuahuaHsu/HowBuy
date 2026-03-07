namespace ISpanShop.MVC.Areas.Admin.Models.Categories
{
    // 分類規格綁定管理頁面 ViewModel
    public class CategorySpecBindingVm
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // 所有可用規格的勾選清單
        public List<SpecCheckItem> Specs { get; set; } = new();
    }

    public class SpecCheckItem
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        // 此規格下的選項預覽（最多顯示5個）
        public List<string> OptionPreview { get; set; } = new();
        // 此分類是否已勾選此規格
        public bool IsBound { get; set; }
        // 排序（在此分類下的顯示順序）
        public int SortOrder { get; set; }
    }
}

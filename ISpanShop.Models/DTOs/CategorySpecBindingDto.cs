namespace ISpanShop.Models.DTOs
{
    public class CategorySpecBindingDto
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public List<string> OptionPreview { get; set; } = new();
        public bool IsBound { get; set; }
        public int SortOrder { get; set; }
    }
}

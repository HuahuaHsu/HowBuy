namespace ISpanShop.Models.DTOs
{
    public class ToggleFilterableDto
    {
        public int CategoryId { get; set; }
        public int SpecId { get; set; }
        public bool IsFilterable { get; set; }
    }
}

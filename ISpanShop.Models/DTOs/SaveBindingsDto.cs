namespace ISpanShop.Models.DTOs
{
    public class SaveBindingsDto
    {
        public int CategoryId { get; set; }
        public List<int> AttributeIds { get; set; } = new();
    }
}

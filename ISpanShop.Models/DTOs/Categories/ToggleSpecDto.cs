namespace ISpanShop.Models.DTOs.Categories
{
    public class ToggleSpecDto
    {
        public int CategoryId  { get; set; }
        public int AttributeId { get; set; }
        public bool IsBound    { get; set; }
    }
}

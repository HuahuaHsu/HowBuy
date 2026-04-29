namespace ISpanShop.Models.DTOs.Auth;

public class OAuthCallbackDto
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}
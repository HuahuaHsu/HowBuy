namespace ISpanShop.Models.DTOs.Auth;

public class OAuthResultDto
{
    public string Status { get; set; } = string.Empty; // "Success" / "MergeRequired"
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? ProviderId { get; set; } // 補上這個，用於合併
}
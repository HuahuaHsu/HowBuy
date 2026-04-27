namespace ISpanShop.Models.DTOs.Auth;

public class OAuthMergeDto
{
    public string Account { get; set; } = string.Empty; // 既有帳號 (Email/Account)
    public string Password { get; set; } = string.Empty; // 既有帳號密碼
    public string Provider { get; set; } = string.Empty; // Google
    public string OAuthProviderId { get; set; } = string.Empty; // 真正的 Google ID
}
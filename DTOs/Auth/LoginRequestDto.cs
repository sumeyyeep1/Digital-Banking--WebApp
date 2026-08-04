namespace DigitalBanking.API.DTOs.Auth;

// Kullanıcının giriş yaparken gönderdiği veri
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

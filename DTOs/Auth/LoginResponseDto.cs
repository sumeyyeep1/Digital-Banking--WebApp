namespace DigitalBanking.API.DTOs.Auth;

// Giriş başarılı olduğunda kullanıcıya döndürülen veri
public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    // JWT Token: Kullanıcının kimliğini kanıtlayan şifreli anahtar
    // Her istekte bu token Authorization header'ında gönderilir
    public string Token { get; set; } = string.Empty;

    // Token ne zaman geçersiz olacak?
    public DateTime TokenExpiry { get; set; }

    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}



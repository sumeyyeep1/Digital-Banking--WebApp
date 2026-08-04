using DigitalBanking.API.DTOs.Auth; // LoginRequestDto ve RegisterRequestDto siniflarini kullanmak icin eklenir.
using DigitalBanking.API.Interfaces; // IAuthService sozlesmesini kullanmak icin eklenir.
using Microsoft.AspNetCore.Mvc; // ControllerBase, IActionResult, HttpPost gibi ASP.NET Core API parcalarini getirir.

namespace DigitalBanking.API.Controllers; // Bu controller'in Controllers alaninda oldugunu soyler.

[ApiController] // Bu sinifin API controller oldugunu soyler; model dogrulama gibi isleri otomatiklestirir.
[Route("api/[controller]")] // [controller] kelimesi AuthController adindan "auth" olarak uretilir; adres /api/auth olur.
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService; // Controller isleri kendi yapmaz; AuthService'e devreder.

    public AuthController(IAuthService authService) // Constructor: Controller olusurken ihtiyac duydugu servisi alir.
    {
        _authService = authService; // Dependency Injection, hazir servisi constructor uzerinden bize verir.
    }

    // POST /api/auth/login adresine gelen giris isteklerini bu metot karsilar.
    [HttpPost("login")] // HTTP POST isteginin "login" yoluna baglanir.
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request) // FromBody, JSON verisinin request nesnesine cevrilecegini soyler.
    {
        var result = await _authService.LoginAsync(request); // Giris kontrolunu servis katmanina yaptiririz.

        if (!result.IsSuccess) // Servis "basarisiz" dediyse kullanici dogrulanamamis demektir.
            return Unauthorized(result); // Basarisiz giriste 401 Unauthorized doneriz.

        return Ok(result); // Basarili giriste 200 OK ve token bilgisini doneriz.
    }

    // POST /api/auth/register adresine gelen kayit isteklerini bu metot karsilar.
    [HttpPost("register")] // HTTP POST isteginin "register" yoluna baglanir.
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request) // JSON kayit formunu RegisterRequestDto nesnesine cevirir.
    {
        var result = await _authService.RegisterAsync(request); // Kayit isini AuthService'e devrederiz.

        if (!result.IsSuccess) // Servis "basarisiz" dediyse is kurali veya veri hatasi vardir.
            return BadRequest(result); // Kullanici hatali veri gonderdiyse 400 Bad Request doneriz.

        return Ok(result); // Kayit basariliysa 200 OK ve token bilgisini doneriz.
    }
}

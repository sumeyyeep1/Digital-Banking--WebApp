using DigitalBanking.API.DTOs.Auth; // Login ve register icin kullanilan DTO siniflarini getirir.

namespace DigitalBanking.API.Interfaces; // Bu dosyanin Interfaces klasoruyle ayni mantiksal alanda oldugunu soyler.

// Interface: Bir sinifin hangi isleri yapacagini soyleyen sozlesmedir.
// Benzetme: Restoranda menu gibidir; ne siparis edilebilecegini soyler, yemegin nasil yapildigini soylemez.
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request); // Login istegini isleyen metot imzasi.
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request); // Register istegini isleyen metot imzasi.
}

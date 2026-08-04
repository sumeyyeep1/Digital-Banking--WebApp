using System.ComponentModel.DataAnnotations; // Form alanlari icin basit dogrulama kurallari yazmamizi saglar.

namespace DigitalBanking.API.DTOs.Auth; // Bu sinifin Auth DTO klasorune ait oldugunu soyler.

// DTO: Dis dunyadan gelen veriyi tasiyan kucuk bir cantadir.
// RegisterRequestDto: Kullanici kayit olurken API'ye gonderilen form verisidir.
public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email alani zorunludur.")] // Email bos gelirse ASP.NET Core otomatik hata uretir.
    [EmailAddress(ErrorMessage = "Gecerli bir email adresi giriniz.")] // Email bicimi dogru mu diye kontrol eder.
    public string Email { get; set; } = string.Empty; // Kullanici sisteme bu email ile giris yapacak.

    [Required(ErrorMessage = "Sifre alani zorunludur.")] // Sifre bos birakilamaz.
    [MinLength(6, ErrorMessage = "Sifre en az 6 karakter olmalidir.")] // Cok kisa sifreleri engeller.
    public string Password { get; set; } = string.Empty; // Kullanici tarafindan yazilan ham sifredir.

    [Required(ErrorMessage = "Sifre tekrari zorunludur.")] // Sifre tekrari bos birakilamaz.
    public string ConfirmPassword { get; set; } = string.Empty; // Yanlis yazimi yakalamak icin sifrenin ikinci kez yazilmis halidir.

    [Required(ErrorMessage = "Ad alani zorunludur.")] // Musteri adi bos birakilamaz.
    public string FirstName { get; set; } = string.Empty; // Musterinin adi.

    [Required(ErrorMessage = "Soyad alani zorunludur.")] // Musteri soyadi bos birakilamaz.
    public string LastName { get; set; } = string.Empty; // Musterinin soyadi.

    [Required(ErrorMessage = "TC Kimlik No alani zorunludur.")] // Kimlik numarasi bos birakilamaz.
    [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalidir.")] // Basit uzunluk kontrolu yapar.
    public string IdentityNumber { get; set; } = string.Empty; // Musterinin benzersiz kimlik numarasi.

    [Required(ErrorMessage = "Telefon alani zorunludur.")] // Telefon bos birakilamaz.
    public string PhoneNumber { get; set; } = string.Empty; // Musterinin telefon numarasi.

    public string Address { get; set; } = string.Empty; // Adres opsiyonel gibi kullanilabilir; bos gelirse bos metin saklanir.
}

using System.IdentityModel.Tokens.Jwt; // JWT token olusturmak icin kullanilir.
using System.Security.Claims; // Token icine kullanici bilgisi koymak icin kullanilir.
using System.Text; // SecretKey metnini byte dizisine cevirmek icin kullanilir.
using DigitalBanking.API.Data; // AppDbContext sinifina ulasmak icin kullanilir.
using DigitalBanking.API.DTOs.Auth; // Login ve register DTO siniflarini kullanmak icin eklenir.
using DigitalBanking.API.Interfaces; // IAuthService sozlesmesini uygulamak icin eklenir.
using DigitalBanking.API.Models; // User ve Customer modellerini kullanmak icin eklenir.
using Microsoft.AspNetCore.Identity; // PasswordHasher ile sifreyi guvenli hale getirmek icin kullanilir.
using Microsoft.EntityFrameworkCore; // FirstOrDefaultAsync ve AnyAsync gibi EF Core metotlari icin kullanilir.
using Microsoft.IdentityModel.Tokens; // JWT token imzalama anahtari icin kullanilir.

namespace DigitalBanking.API.Services; // Bu sinifin Services alaninda oldugunu soyler.

// AuthService: Giris ve kayit gibi kimlik islemlerinin asil is mantigini tutar.
// Benzetme: Controller musteriyle konusan veznedir; Service arkada isi yapan operasyon ekibidir.
public class AuthService : IAuthService
{
    private readonly AppDbContext _context; // Veritabaniyla konusmak icin kullandigimiz EF Core baglami.
    private readonly IConfiguration _configuration; // appsettings.json icindeki ayarlari okumak icin kullanilir.
    private readonly IPasswordHasher<User> _passwordHasher; // Sifreleri okunamaz hash haline getiren yardimci servis.

    public AuthService( // Dependency Injection bu constructor'a ihtiyac duyulan nesneleri verir.
        AppDbContext context, // EF Core veritabani baglami disaridan gelir.
        IConfiguration configuration, // Uygulama ayarlari disaridan gelir.
        IPasswordHasher<User> passwordHasher) // Sifre hashleme servisi disaridan gelir.
    {
        _context = context; // Gelen veritabani baglamini sinif icinde kullanmak icin saklariz.
        _configuration = configuration; // Gelen ayar okuyucuyu sinif icinde kullanmak icin saklariz.
        _passwordHasher = passwordHasher; // Gelen sifre hashleme servisini sinif icinde kullanmak icin saklariz.
    }

    // LoginAsync: Kullanici email ve sifre gonderdiginde giris yapabilir mi diye kontrol eder.
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users // Users tablosundan arama baslatiriz.
            .FirstOrDefaultAsync(u => u.Email == request.Email); // Email eslesen ilk kullaniciyi buluruz; yoksa null doner.

        if (user == null) // Kullanici bulunamazsa giris devam edemez.
        {
            return new LoginResponseDto // API'ye donulecek cevap nesnesini olustururuz.
            {
                IsSuccess = false, // Islemin basarisiz oldugunu soyleriz.
                Message = "Bu email ile kayitli kullanici bulunamadi." // Kullaniciya okunabilir hata mesaji veririz.
            };
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password); // Girilen sifre hash ile uyusuyor mu bakariz.

        if (passwordResult == PasswordVerificationResult.Failed) // Sifre dogru degilse girisi reddederiz.
        {
            return new LoginResponseDto // Hata cevabi olustururuz.
            {
                IsSuccess = false, // Islemin basarisiz oldugunu belirtiriz.
                Message = "Sifre hatali." // Kullaniciya sade hata mesaji veririz.
            };
        }

        var token = CreateToken(user); // Kullanici dogrulandiysa ona JWT token uretiriz.

        return new LoginResponseDto // Basarili giris cevabini olustururuz.
        {
            UserId = user.Id, // Kullanici id bilgisini doneriz.
            Email = user.Email, // Kullanici email bilgisini doneriz.
            Role = user.Role.ToString(), // Enum olan rolu metin olarak doneriz.
            Token = token.TokenString, // Uretilen JWT token metnini doneriz.
            TokenExpiry = token.ExpiryDate, // Token gecerlilik bitis tarihini doneriz.
            IsSuccess = true, // Islemin basarili oldugunu soyleriz.
            Message = "Giris basarili." // Kullaniciya basari mesaji veririz.
        };
    }

    // RegisterAsync: Yeni kullanici ve ona bagli musteri kaydi olusturur.
    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (request.Password != request.ConfirmPassword) // Sifre ve sifre tekrari ayni degilse kayit yapmayiz.
        {
            return new LoginResponseDto // Hata cevabi olustururuz.
            {
                IsSuccess = false, // Islemin basarisiz oldugunu belirtiriz.
                Message = "Sifreler eslesmiyor." // Kullaniciya neyin yanlis oldugunu soyleriz.
            };
        }

        var emailAlreadyExists = await _context.Users // Users tablosunda kontrol baslatiriz.
            .AnyAsync(u => u.Email == request.Email); // Bu email daha once kullanilmis mi diye bakariz.

        if (emailAlreadyExists) // Email zaten varsa ayni email ile ikinci hesap acmayiz.
        {
            return new LoginResponseDto // Hata cevabi olustururuz.
            {
                IsSuccess = false, // Islemin basarisiz oldugunu belirtiriz.
                Message = "Bu email adresi zaten kullaniliyor." // Kullaniciya emailin tekrarli oldugunu soyleriz.
            };
        }

        var identityNumberAlreadyExists = await _context.Customers // Customers tablosunda kontrol baslatiriz.
            .AnyAsync(c => c.IdentityNumber == request.IdentityNumber); // Bu TC Kimlik No daha once kullanilmis mi diye bakariz.

        if (identityNumberAlreadyExists) // Kimlik numarasi zaten varsa ikinci musteri acmayiz.
        {
            return new LoginResponseDto // Hata cevabi olustururuz.
            {
                IsSuccess = false, // Islemin basarisiz oldugunu belirtiriz.
                Message = "Bu TC Kimlik No ile kayitli musteri zaten var." // Kullaniciya kimlik numarasinin tekrarli oldugunu soyleriz.
            };
        }

        var newUser = new User // Yeni User nesnesi olustururuz; bu Users tablosuna gidecek.
        {
            Email = request.Email.Trim(), // Email etrafindaki gereksiz bosluklari temizleyip saklariz.
            Role = DigitalBanking.API.Enums.UserRole.Customer // Register olan kisi varsayilan olarak musteri rolundedir.
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password); // Ham sifreyi okunamaz hash haline cevirip saklariz.

        var newCustomer = new Customer // Yeni Customer nesnesi olustururuz; bu Customers tablosuna gidecek.
        {
            FirstName = request.FirstName.Trim(), // Ad bilgisini temizleyip saklariz.
            LastName = request.LastName.Trim(), // Soyad bilgisini temizleyip saklariz.
            IdentityNumber = request.IdentityNumber.Trim(), // TC Kimlik No bilgisini temizleyip saklariz.
            PhoneNumber = request.PhoneNumber.Trim(), // Telefon bilgisini temizleyip saklariz.
            Address = request.Address.Trim(), // Adres bilgisini temizleyip saklariz.
            User = newUser // Customer kaydini yeni User kaydina baglariz; EF Core UserId'yi otomatik ayarlar.
        };

        _context.Users.Add(newUser); // Yeni kullaniciyi EF Core'un takip listesine ekleriz.
        _context.Customers.Add(newCustomer); // Yeni musteri bilgisini EF Core'un takip listesine ekleriz.

        await _context.SaveChangesAsync(); // Takip listesindeki User ve Customer kayitlarini veritabanina yazariz.

        var token = CreateToken(newUser); // Kayit basarili olunca kullaniciyi tekrar login ettirmeden token uretiriz.

        return new LoginResponseDto // Basarili kayit cevabini olustururuz.
        {
            UserId = newUser.Id, // Yeni kullanicinin veritabanindan gelen id bilgisini doneriz.
            Email = newUser.Email, // Yeni kullanicinin email bilgisini doneriz.
            Role = newUser.Role.ToString(), // Kullanici rolunu metin olarak doneriz.
            Token = token.TokenString, // Uretilen JWT token metnini doneriz.
            TokenExpiry = token.ExpiryDate, // Token gecerlilik bitis tarihini doneriz.
            IsSuccess = true, // Islemin basarili oldugunu soyleriz.
            Message = "Kayit basarili." // Kullaniciya basari mesaji veririz.
        };
    }

    // CreateToken: Kullanici bilgilerini iceren JWT token uretir.
    private (string TokenString, DateTime ExpiryDate) CreateToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]!; // Token imzalamak icin gizli anahtari okuruz.
        var issuer = _configuration["JwtSettings:Issuer"]!; // Token'i hangi uygulama uretti bilgisini okuruz.
        var audience = _configuration["JwtSettings:Audience"]!; // Token'i hangi istemciler kullanabilir bilgisini okuruz.
        var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!); // Token kac dakika gecerli olacak bilgisini okuruz.

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); // Gizli metni imzalama anahtarina ceviririz.
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Token'i hangi algoritmayla imzalayacagimizi soyleriz.

        var claims = new[] // Claim: Token icine koyulan kucuk kimlik karti bilgileri gibidir.
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanici id bilgisini token'a ekleriz.
            new Claim(ClaimTypes.Email, user.Email), // Kullanici email bilgisini token'a ekleriz.
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Kullanici rol bilgisini token'a ekleriz.
        };

        var expiryDate = DateTime.UtcNow.AddMinutes(expiryMinutes); // Token'in ne zaman bitecegini hesaplariz.

        var token = new JwtSecurityToken( // Yeni JWT token nesnesi olustururuz.
            issuer: issuer, // Token'i ureteren uygulama bilgisini koyariz.
            audience: audience, // Token'in hedef kullanici/istemci bilgisini koyariz.
            claims: claims, // Token icindeki kimlik bilgilerini koyariz.
            expires: expiryDate, // Token'in son kullanma tarihini koyariz.
            signingCredentials: signingCredentials // Token'in imzasini koyariz.
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token); // Token nesnesini API cevabinda donulecek metne ceviririz.

        return (tokenString, expiryDate); // Hem token metnini hem de bitis tarihini birlikte doneriz.
    }
}

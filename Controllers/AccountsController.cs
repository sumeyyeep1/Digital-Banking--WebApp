using DigitalBanking.API.DTOs.Accounts;
using DigitalBanking.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalBanking.API.Controllers;

[ApiController] //Bu sýnýfýýn bir api controller olduðunu belirtir. Bu attribute, model binding, validation ve response formatting gibi özellikleri etkinleþtirir.
[Authorize] //Anlamý bu apiyi sadece giriþ yapan kullanýcýlar kullanabilir. Giriþ yapmamýþ kullanýcýlar bu apiyi kullanamaz.
[Route("api/[controller]")] //Controller adresini belirtir. [controller] kýsmý, controller sýnýfýnýn adýný temsil eder. Örneðin, bu controller'ýn adý AccountsController olduðundan, route "api/accounts" olur.
//Bu controller, hesaplarla ilgili iþlemleri yönetir. Hesap oluþturma, güncelleme ve kullanýcýya ait hesaplarý listeleme gibi iþlemleri içerir.Ve controllerBase sýnýfýndan türetilmiþtir.
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;// IaccountService türünde field tanýmlar.Ve bu alan hesaplarla ilgili iþlemleri gerçekleþtirmek için kullanýlýr.

    public AccountsController(IAccountService accountService) //IAccountService ýn constructorý ve dependency injection ile IAccountService türünde bir nesne alýr ve _accountService alanýna atar.
    {
        _accountService = accountService;// Bu controller içinde bu nesneyle ilgili iþlemler yapmak için oluþan nesneyi fielda atar.
    }

    [HttpGet("my")] //  Bu endpointe GET isteði gönderildiðinde, kullanýcýya ait hesaplar listelenir.My yazýsý adres sonuna eklenir. Örneðin, api/accounts/my adresine GET isteði gönderildiðinde bu metod çalýþýr.
    public async Task<IActionResult> GetMyAccounts()// Kullanýcýya ait hesaplarý listeler geri Iactionresult döndürür. 
    {
        var userId = GetCurrentUserId(); // Kullanýcýnýn kimliðini alýr. GetCurrentUserId metodu, kullanýcýnýn kimliðini JWT tokenýndan veya oturumdan alýr.

        if (userId == null)
        {
            return Unauthorized();// UserId null ise bu isteði yapmaya yetki yoktur. Bu yüzden bu metot çaðýrýlýr.
        }

        var accounts = await _accountService.GetMyAccountsAsync(userId.Value);//Null deðilse , _accountService üzerinden GetMyAccountsAsync metodunu çaðýrýr ve kullanýcýya ait hesaplarý alýr.
        return Ok(accounts);// Hesaplarý ve 200 durum kodunu döndürür.
    }

    [HttpPost] //Bu istek endpointe gönderilir.Veri eklemek için kullanýlýr.
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto request)// FromBody ile jsondaki veriyi alýr dto ya dönüþtürür.Ve IActionresult döndürür. request dto classýndan türetilmiþtir.Yani hesap oluþturma isteði gelirse request ile bilgiler tutulur.
    {
        var userId = GetCurrentUserId(); //Kullanýcýnýn bilgilerini oturumdan veya token üzerinden alýr .
        if (userId == null)
        {
            return Unauthorized();// Bu iþlemi yapmaya yetkin yok der ve 401 döndürür.
        }

        var account = await _accountService.CreateAccountAsync(userId.Value, request); // Burada interfacede tanýmlanan CreateAccountAsync metodu çaðýrýlýr ve bu metodun yapacaðý iþlemleri biz yazarýz.

        if (account == null)
        {
            return BadRequest(new { message = "Musteri kaydi bulunamadi." }); // bad request istenilen bilgi bulunamayýnca döndürülür. 
        }

        return Ok(account);// Hesap oluþturulduysa 200 durum kodu ve hesap bilgilerini döndürür.
    }

    [HttpPut("{accountId:int}")]// BU metodun bir güncelleme yapacaðýný ve url de accountId parametresini alacaðýný söyler. Örneðin, api/accounts/5 adresine PUT isteði gönderildiðinde accountId 5 olur ve bu metod çalýþýr.   
    public async Task<IActionResult> UpdateAccount(int accountId, [FromBody] UpdateAccountRequestDto request) // accountId parametresi url den alýnýr ve request dto classýndan türetilir. Bu dto güncelleme isteði gelirse kullanýcýnýn gönderdiði bilgileri tutar.
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var account = await _accountService.UpdateAccountAsync(userId.Value, accountId, request);

        if (account == null)
        {
            return NotFound(new { message = "Hesap bulunamadi veya bu hesaba erisim yetkin yok." });// 404 db de bu kayýt bulunamadýðýnda veya kullanýcýya ait olmayan bir hesaba eriþmeye çalýþtýðýnda döndürülür.
        }

        return Ok(account); // Hesap güncellendiyse 200 durum kodu ve güncellenmiþ hesap bilgilerini döndürür.
    }

    private int? GetCurrentUserId() // 
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);// Kullaýcý giriþ yaptýktan sonra bilgiler jwt tokendan alýnýr. claim olarak saklanýr. ClaimTypes.NameIdentifier kullanýcýnýn id sini alýr. Bu idyi userIdClaim deðiþkenine atar.
        return int.TryParse(userIdClaim, out var userId) ? userId : null;// userIdClaim deðiþkeni int türüne dönüþtürülmeye çalýþýlýr. Dönüþtürülebilirse userId deðiþkenine atanýr ve döndürülür. Dönüþtürülemezse null döndürülür.
    }
}

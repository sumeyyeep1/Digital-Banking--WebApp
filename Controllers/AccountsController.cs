using System.Security.Claims;
using DigitalBanking.API.DTOs.Accounts;
using DigitalBanking.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBanking.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAccounts()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var accounts = await _accountService.GetMyAccountsAsync(userId.Value);
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var account = await _accountService.CreateAccountAsync(userId.Value, request);

        if (account == null)
        {
            return BadRequest(new { message = "Musteri kaydi bulunamadi." });
        }

        return Ok(account);
    }

    [HttpPut("{accountId:int}")]
    public async Task<IActionResult> UpdateAccount(int accountId, [FromBody] UpdateAccountRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var account = await _accountService.UpdateAccountAsync(userId.Value, accountId, request);

        if (account == null)
        {
            return NotFound(new { message = "Hesap bulunamadi veya bu hesaba erisim yetkin yok." });
        }

        return Ok(account);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

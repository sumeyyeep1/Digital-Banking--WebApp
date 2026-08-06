using DigitalBanking.API.Data;
using DigitalBanking.API.DTOs.Accounts;
using DigitalBanking.API.Enums;
using DigitalBanking.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.API.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountResponseDto>> GetMyAccountsAsync(int userId)
    {
        return await _context.Accounts
            .Where(a => a.Customer.UserId == userId && a.Status == EntityStatus.Active)
            .Select(a => new AccountResponseDto
            {
                Id = a.Id,
                Iban = a.Iban,
                AccountType = a.AccountType.ToString(),
                Currency = a.Currency.ToString(),
                Balance = a.Balance
            })
            .ToListAsync();
    }
}

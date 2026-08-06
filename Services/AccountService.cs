using DigitalBanking.API.Data;
using DigitalBanking.API.DTOs.Accounts;
using DigitalBanking.API.Enums;
using DigitalBanking.API.Interfaces;
using DigitalBanking.API.Models;
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

    public async Task<AccountResponseDto?> CreateAccountAsync(int userId, CreateAccountRequestDto request)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == EntityStatus.Active);

        if (customer == null)
        {
            return null;
        }

        var account = new Account
        {
            CustomerId = customer.Id,
            AccountType = request.AccountType,
            Currency = request.Currency,
            Iban = await GenerateUniqueIbanAsync()
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return MapToResponse(account);
    }

    public async Task<AccountResponseDto?> UpdateAccountAsync(int userId, int accountId, UpdateAccountRequestDto request)
    {
        var account = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.Customer.UserId == userId && a.Status == EntityStatus.Active);

        if (account == null)
        {
            return null;
        }

        account.AccountType = request.AccountType;
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(account);
    }

    private async Task<string> GenerateUniqueIbanAsync()
    {
        string iban;

        do
        {
            iban = $"TR{Random.Shared.NextInt64(10_000_000_000_000_000, 99_999_999_999_999_999)}";
        }
        while (await _context.Accounts.AnyAsync(a => a.Iban == iban));

        return iban;
    }

    private static AccountResponseDto MapToResponse(Account account)
    {
        return new AccountResponseDto
        {
            Id = account.Id,
            Iban = account.Iban,
            AccountType = account.AccountType.ToString(),
            Currency = account.Currency.ToString(),
            Balance = account.Balance
        };
    }
}

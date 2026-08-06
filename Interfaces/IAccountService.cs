using DigitalBanking.API.DTOs.Accounts;

namespace DigitalBanking.API.Interfaces;

public interface IAccountService
{
    Task<List<AccountResponseDto>> GetMyAccountsAsync(int userId);
    Task<AccountResponseDto?> CreateAccountAsync(int userId, CreateAccountRequestDto request);
    Task<AccountResponseDto?> UpdateAccountAsync(int userId, int accountId, UpdateAccountRequestDto request);
}

using DigitalBanking.API.DTOs.Accounts;

namespace DigitalBanking.API.Interfaces;

public interface IAccountService
{
    Task<List<AccountResponseDto>> GetMyAccountsAsync(int userId);
}

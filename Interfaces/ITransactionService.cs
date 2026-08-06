using DigitalBanking.API.DTOs.Transactions;

namespace DigitalBanking.API.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponseDto> DepositAsync(int userId, DepositRequestDto request);
    Task<TransactionResponseDto> WithdrawAsync(int userId, WithdrawRequestDto request);
    Task<TransactionResponseDto> TransferAsync(int userId, TransferRequestDto request);
}

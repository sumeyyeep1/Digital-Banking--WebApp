using DigitalBanking.API.Data;
using DigitalBanking.API.DTOs.Transactions;
using DigitalBanking.API.Enums;
using DigitalBanking.API.Interfaces;
using DigitalBanking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.API.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionResponseDto> DepositAsync(int userId, DepositRequestDto request)
    {
        var account = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.Customer.UserId == userId && a.Status == EntityStatus.Active);

        if (account == null)
        {
            return Fail("Hesap bulunamadi veya bu hesaba erisim yetkin yok.");
        }

        if (request.Amount <= 0)
        {
            return Fail("Tutar 0'dan buyuk olmalidir.");
        }

        account.Balance += request.Amount;
        account.UpdatedAt = DateTime.UtcNow;

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Deposit,
            Amount = request.Amount,
            Description = request.Description,
            ReceiverAccountId = account.Id
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Success("Para yatirma islemi basarili.", transaction.Id, account.Balance);
    }

    public async Task<TransactionResponseDto> TransferAsync(int userId, TransferRequestDto request)
    {
        if (request.Amount <= 0)
        {
            return Fail("Tutar 0'dan buyuk olmalidir.");
        }

        var normalizedReceiverIban = request.ReceiverIban.Trim().ToUpperInvariant();

        await using var dbTransaction = await _context.Database.BeginTransactionAsync();

        var senderAccount = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == request.SenderAccountId && a.Customer.UserId == userId && a.Status == EntityStatus.Active);

        if (senderAccount == null)
        {
            return Fail("Gonderen hesap bulunamadi veya bu hesaba erisim yetkin yok.");
        }

        var receiverAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Iban == normalizedReceiverIban && a.Status == EntityStatus.Active);

        if (receiverAccount == null)
        {
            return Fail("Alici IBAN bulunamadi.");
        }

        if (senderAccount.Id == receiverAccount.Id)
        {
            return Fail("Ayni hesaba para gonderilemez.");
        }

        if (senderAccount.Currency != receiverAccount.Currency)
        {
            return Fail("Farkli para birimleri arasinda transfer desteklenmiyor.");
        }

        if (senderAccount.Balance < request.Amount)
        {
            return Fail("Yetersiz bakiye.");
        }

        senderAccount.Balance -= request.Amount;
        receiverAccount.Balance += request.Amount;
        senderAccount.UpdatedAt = DateTime.UtcNow;
        receiverAccount.UpdatedAt = DateTime.UtcNow;

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Transfer,
            Amount = request.Amount,
            Description = request.Description,
            SenderAccountId = senderAccount.Id,
            ReceiverAccountId = receiverAccount.Id
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        await dbTransaction.CommitAsync();

        return Success("Para transferi basarili.", transaction.Id, senderAccount.Balance);
    }

    private static TransactionResponseDto Fail(string message)
    {
        return new TransactionResponseDto
        {
            IsSuccess = false,
            Message = message
        };
    }

    private static TransactionResponseDto Success(string message, int transactionId, decimal currentBalance)
    {
        return new TransactionResponseDto
        {
            IsSuccess = true,
            Message = message,
            TransactionId = transactionId,
            CurrentBalance = currentBalance
        };
    }
}

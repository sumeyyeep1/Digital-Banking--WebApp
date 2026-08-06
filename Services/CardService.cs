using System.Security.Cryptography;
using System.Text;
using DigitalBanking.API.Data;
using DigitalBanking.API.DTOs.Cards;
using DigitalBanking.API.Enums;
using DigitalBanking.API.Interfaces;
using DigitalBanking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.API.Services;

public class CardService : ICardService
{
    private readonly AppDbContext _context;

    public CardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CardResponseDto>> GetMyCardsAsync(int userId)
    {
        return await _context.Cards
            .Include(c => c.Account)
            .Where(c => c.Account.Customer.UserId == userId && c.Status == EntityStatus.Active && c.Account.Status == EntityStatus.Active)
            .Select(c => new CardResponseDto
            {
                Id = c.Id,
                MaskedCardNumber = MaskCardNumber(c.CardNumber),
                CardHolderName = c.CardHolderName,
                ExpiryMonth = c.ExpiryMonth,
                ExpiryYear = c.ExpiryYear,
                CardType = c.CardType.ToString(),
                AccountId = c.AccountId,
                AccountIban = c.Account.Iban
            })
            .ToListAsync();
    }

    public async Task<CardOperationResponseDto> CreateCardAsync(int userId, CreateCardRequestDto request)
    {
        var account = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.Customer.UserId == userId && a.Status == EntityStatus.Active);

        if (account == null)
        {
            return Fail("Hesap bulunamadi veya bu hesaba erisim yetkin yok.");
        }

        var expiry = DateTime.UtcNow.AddYears(4);
        var card = new Card
        {
            AccountId = account.Id,
            CardHolderName = request.CardHolderName.Trim().ToUpperInvariant(),
            CardType = request.CardType,
            CardNumber = await GenerateUniqueCardNumberAsync(),
            ExpiryMonth = expiry.Month.ToString("D2"),
            ExpiryYear = expiry.Year.ToString(),
            CvvHash = HashValue(Random.Shared.Next(100, 1000).ToString())
        };

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        return Success("Kart olusturma islemi basarili.", card.Id);
    }

    public async Task<CardOperationResponseDto> UpdateCardAsync(int userId, int cardId, UpdateCardRequestDto request)
    {
        var card = await _context.Cards
            .Include(c => c.Account)
            .ThenInclude(a => a.Customer)
            .FirstOrDefaultAsync(c => c.Id == cardId && c.Account.Customer.UserId == userId && c.Status == EntityStatus.Active);

        if (card == null)
        {
            return Fail("Kart bulunamadi veya bu karta erisim yetkin yok.");
        }

        card.CardHolderName = request.CardHolderName.Trim().ToUpperInvariant();
        card.CardType = request.CardType;
        card.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Success("Kart guncelleme islemi basarili.", card.Id);
    }

    private async Task<string> GenerateUniqueCardNumberAsync()
    {
        string cardNumber;

        do
        {
            cardNumber = $"4444{Random.Shared.NextInt64(1000_0000_0000, 9999_9999_9999)}";
        }
        while (await _context.Cards.AnyAsync(c => c.CardNumber == cardNumber));

        return cardNumber;
    }

    private static string MaskCardNumber(string cardNumber)
    {
        return cardNumber.Length < 4 ? "****" : $"**** **** **** {cardNumber[^4..]}";
    }

    private static string HashValue(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }

    private static CardOperationResponseDto Fail(string message)
    {
        return new CardOperationResponseDto
        {
            IsSuccess = false,
            Message = message
        };
    }

    private static CardOperationResponseDto Success(string message, int cardId)
    {
        return new CardOperationResponseDto
        {
            IsSuccess = true,
            Message = message,
            CardId = cardId
        };
    }
}

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
        return await _context.Cards // db den cards tablosunu al
            .Include(c => c.Account) // cards tablosundaki accountId ile accounts tablosunu join et
            .Where(c => c.Account.Customer.UserId == userId && c.Status == EntityStatus.Active && c.Account.Status == EntityStatus.Active)// sadece aktif kartlarý ve aktif hesaplarý filtrele
            .Select(c => new CardResponseDto  //Filtrelenen her kart kaydýný dto nesnesine dönüþtürür.
            { 
                Id = c.Id,
                MaskedCardNumber = MaskCardNumber(c.CardNumber), // kart numarasýný maskeler
                CardHolderName = c.CardHolderName,
                ExpiryMonth = c.ExpiryMonth,
                ExpiryYear = c.ExpiryYear,
                CardType = c.CardType.ToString(),
                AccountId = c.AccountId,
                AccountIban = c.Account.Iban
            })
            .ToListAsync();// dto nesnelerini liste olarak döndürür
    }

    public async Task<CardOperationResponseDto> CreateCardAsync(int userId, CreateCardRequestDto request) 
    {
        var account = await _context.Accounts // db den accounts tablosunu al
            .Include(a => a.Customer) // accounts tablosundaki customerId ile customers tablosunu join et
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.Customer.UserId == userId && a.Status == EntityStatus.Active); //

        if (account == null)
        {
            return Fail("Hesap bulunamadi veya bu hesaba erisim yetkin yok.");
        }

        var expiry = DateTime.UtcNow.AddYears(4); // Son kullanma tarihi olarak 4 yýl sonrasýný belirler
        var card = new Card // yeni bir kart nesnesi oluþturur
        {
            AccountId = account.Id,
            CardHolderName = request.CardHolderName.Trim().ToUpperInvariant(),
            CardType = request.CardType,
            CardNumber = await GenerateUniqueCardNumberAsync(),
            ExpiryMonth = expiry.Month.ToString("D2"),
            ExpiryYear = expiry.Year.ToString(),
            CvvHash = HashValue(Random.Shared.Next(100, 1000).ToString())
        };

        _context.Cards.Add(card); // kart nesnesini db ye ekler
        await _context.SaveChangesAsync(); // db ye kaydeder

        return Success("Kart olusturma islemi basarili.", card.Id); // baþarýlý bir þekilde kart oluþturulduðunu ve kartýn id'sini döndürür
    }

    public async Task<CardOperationResponseDto> UpdateCardAsync(int userId, int cardId, UpdateCardRequestDto request)
    {
        var card = await _context.Cards
            .Include(c => c.Account) // cards tablosundaki accountId ile accounts tablosunu join et
            .ThenInclude(a => a.Customer)// accounts tablosundaki customerId ile customers tablosunu join et        
            .FirstOrDefaultAsync(c => c.Id == cardId && c.Account.Customer.UserId == userId && c.Status == EntityStatus.Active);

        if (card == null)
        {
            return Fail("Kart bulunamadi veya bu karta erisim yetkin yok.");
        }

        card.CardHolderName = request.CardHolderName.Trim().ToUpperInvariant(); // kart sahibinin adýný günceller
        card.CardType = request.CardType; // kart tipini günceller
        card.UpdatedAt = DateTime.UtcNow; // güncelleme tarihini günceller

        await _context.SaveChangesAsync(); //deðiþiklikleri kaydeder.

        return Success("Kart guncelleme islemi basarili.", card.Id); //kart id ve mesajý döndürür.
    }

    private async Task<string> GenerateUniqueCardNumberAsync() // yeni bir kart numarasý oluþturur ve veritabanýnda benzersiz olmasýný saðlar.
    {
        string cardNumber;

        do
        {
            cardNumber = $"4444{Random.Shared.NextInt64(1000_0000_0000, 9999_9999_9999)}"; // kart numarasýnýn ilk 4 hanesi 4444 olarak belirlenir ve geri kalan 12 hane rastgele oluþturulur.
        }
        while (await _context.Cards.AnyAsync(c => c.CardNumber == cardNumber)); // AnyAsync metodu, veritabanýnda bu kart numarasý ile eþleþen herhangi bir kart olup olmadýðýný kontrol eder.

        return cardNumber; // benzersiz kart numarasýný döndürür.
    }

    private static string MaskCardNumber(string cardNumber) // kart numarasýný maskeler ve sadece son 4 hanesini gösterir.
    {
        return cardNumber.Length < 4 ? "****" : $"**** **** **** {cardNumber[^4..]}"; 
    }

    private static string HashValue(string value) // verilen deðeri SHA256 algoritmasý ile hashler ve base64 formatýnda döndürür.
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }

    private static CardOperationResponseDto Fail(string message) // baþarýsýz bir iþlem durumunu temsil eden CardOperationResponseDto nesnesi döndürür.
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

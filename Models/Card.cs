using DigitalBanking.API.Enums;

namespace DigitalBanking.API.Models;

public class Card : BaseEntity
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string CvvHash { get; set; } = string.Empty;
    public CardType CardType { get; set; } = CardType.Debit;

    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
}

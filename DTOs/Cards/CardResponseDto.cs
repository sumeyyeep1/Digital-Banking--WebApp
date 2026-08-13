namespace DigitalBanking.API.DTOs.Cards;
// Kart bilgilerini döndüren DTO
public class CardResponseDto
{
    public int Id { get; set; }
    public string MaskedCardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public string AccountIban { get; set; } = string.Empty;
}

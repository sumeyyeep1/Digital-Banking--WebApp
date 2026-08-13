namespace DigitalBanking.API.DTOs.Cards;
// Kart iþlemleri için kullanýlan DTO
public class CardOperationResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? CardId { get; set; }
}

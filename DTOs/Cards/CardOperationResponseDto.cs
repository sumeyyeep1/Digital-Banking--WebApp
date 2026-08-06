namespace DigitalBanking.API.DTOs.Cards;

public class CardOperationResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? CardId { get; set; }
}

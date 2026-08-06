namespace DigitalBanking.API.DTOs.Transactions;

public class TransactionResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? TransactionId { get; set; }
    public decimal? CurrentBalance { get; set; }
}

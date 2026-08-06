using System.ComponentModel.DataAnnotations;

namespace DigitalBanking.API.DTOs.Transactions;

public class TransferRequestDto
{
    [Required]
    public int SenderAccountId { get; set; }

    [Required]
    public string ReceiverIban { get; set; } = string.Empty;

    [Range(0.01, 1_000_000, ErrorMessage = "Tutar 0'dan buyuk olmalidir.")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }
}

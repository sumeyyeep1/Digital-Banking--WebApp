using System.ComponentModel.DataAnnotations;
using DigitalBanking.API.Enums;

namespace DigitalBanking.API.DTOs.Cards;

public class UpdateCardRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CardHolderName { get; set; } = string.Empty;

    [Required]
    public CardType CardType { get; set; }
}

using System.ComponentModel.DataAnnotations;
using DigitalBanking.API.Enums;

namespace DigitalBanking.API.DTOs.Cards;
// Yeni kart oluþturmak için kullanýlan DTO (Data Transfer Object) sýnýfý
public class CreateCardRequestDto
{
    [Required]
    public int AccountId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CardHolderName { get; set; } = string.Empty;

    [Required]
    public CardType CardType { get; set; } = CardType.Debit;
}

using System.ComponentModel.DataAnnotations;
using DigitalBanking.API.Enums;

namespace DigitalBanking.API.DTOs.Accounts;

public class CreateAccountRequestDto
{
    [Required]
    public AccountType AccountType { get; set; } = AccountType.Checking;

    [Required]
    public Currency Currency { get; set; } = Currency.TRY;
}

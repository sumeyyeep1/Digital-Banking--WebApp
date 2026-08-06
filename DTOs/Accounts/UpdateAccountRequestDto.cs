using System.ComponentModel.DataAnnotations;
using DigitalBanking.API.Enums;

namespace DigitalBanking.API.DTOs.Accounts;

public class UpdateAccountRequestDto
{
    [Required]
    public AccountType AccountType { get; set; }
}

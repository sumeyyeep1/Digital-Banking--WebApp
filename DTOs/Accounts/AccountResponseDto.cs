namespace DigitalBanking.API.DTOs.Accounts;

public class AccountResponseDto
{
    public int Id { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

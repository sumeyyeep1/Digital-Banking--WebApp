using DigitalBanking.API.Enums;

namespace DigitalBanking.API.Models;

// Banka hesaplarını temsil eder
public class Account : BaseEntity
{
    // IBAN (benzersiz olmalı)
    public string Iban { get; set; } = string.Empty;

    // Hesap türü (Vadesiz, Vadeli, Yatırım)
    public AccountType AccountType { get; set; } = AccountType.Checking;

    // Para birimi (TRY, USD, EUR...)
    public Currency Currency { get; set; } = Currency.TRY;

    // Bakiye - decimal: Kuruş hassasiyetli para tipi (float/double kullanma!)
    public decimal Balance { get; set; } = 0.00m;

    // ----- İLİŞKİLER -----

    // Hangi müşteriye ait? (Foreign Key)
    public int CustomerId { get; set; }

    // Navigation Property: Bağlı olduğu müşteri
    public virtual Customer Customer { get; set; } = null!;

    // Navigation Property: Bu hesabın gönderdiği transferler
    public virtual ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();

    // Navigation Property: Bu hesabın aldığı transferler
    public virtual ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
}

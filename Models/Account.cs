using DigitalBanking.API.Enums;

namespace DigitalBanking.API.Models;

// Banka hesaplarını temsil eder.Fieldlar oluşturulur.
//  Base entityden miras alır, yani Id, CreatedAt, UpdatedAt ve Status alanlarını da içerir.
public class Account : BaseEntity
{
    // IBAN (benzersiz olmalı)
    public string Iban { get; set; } = string.Empty; // Başlangıç değeri boş metin.

    // Hesap türü (Vadesiz, Vadeli, Yatırım)
    public AccountType AccountType { get; set; } = AccountType.Checking;// Varsayılan olarak vadesiz hesap türü atanır.

    // Para birimi (TRY, USD, EUR...)
    public Currency Currency { get; set; } = Currency.TRY;

    // Bakiye - decimal: Kuruş hassasiyetli para tipi (float/double kullanma!)
    public decimal Balance { get; set; } = 0.00m;

    // ----- İLİŞKİLER -----

    // Hangi müşteriye ait? (Foreign Key)
    public int CustomerId { get; set; }

    // Navigation Property: Bağlı olduğu müşteri
    public virtual Customer Customer { get; set; } = null!; // navigation property ile bağlantı kurulur böylece customer classına ulaşabiliriz.

    // Navigation Property: Bu hesabın gönderdiği transferler
    public virtual ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>(); // boş bir liste ile başlar, birden fazla transaction nesnesi tutabilir.
     
    // Navigation Property: Bu hesabın aldığı transferler
    public virtual ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>(); // Bu hesabın bağlı olduğu kartlar
}

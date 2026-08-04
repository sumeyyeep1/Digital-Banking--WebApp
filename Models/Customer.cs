namespace DigitalBanking.API.Models;

// Banka müşterilerinin kişisel bilgilerini tutar
public class Customer : BaseEntity
{
    // Ad ve Soyad
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // TC Kimlik No (benzersiz olmalı)
    public string IdentityNumber { get; set; } = string.Empty;

    // Telefon numarası
    public string PhoneNumber { get; set; } = string.Empty;

    // Adres
    public string Address { get; set; } = string.Empty;

    // ----- İLİŞKİLER -----

    // Hangi User'a bağlı? (Foreign Key)
    public int UserId { get; set; }

    // Navigation Property: Bağlı olduğu kullanıcı
    public virtual User User { get; set; } = null!;

    // Navigation Property: Bu müşteriye ait hesaplar (1 müşteri → N hesap)
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}

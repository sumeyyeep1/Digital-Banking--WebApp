using DigitalBanking.API.Enums;

namespace DigitalBanking.API.Models;

// Sisteme giriş yapan kullanıcıları temsil eder
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    // Şifre düz metin saklanmaz, hash'lenmiş hali saklanır
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Customer;

    // Navigation Property: Bu kullanıcıya ait müşteri bilgileri (1 User → 1 Customer)
    public virtual Customer? Customer { get; set; } // ? ile nullable yapıldı, çünkü her kullanıcı bir müşteri olmayabilir (örneğin banka görevlisi veya admin olabilir)
}

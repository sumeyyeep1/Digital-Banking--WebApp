using DigitalBanking.API.Enums;

namespace DigitalBanking.API.Models;

// Tüm modellerin ortak alanlarını tutan soyut temel sınıf
// abstract: Doğrudan new BaseEntity() yapılamaz, sadece miras alınır
public abstract class BaseEntity
{
    public int Id { get; set; }

    // Kaydın ne zaman oluşturulduğu
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Kaydın ne zaman güncellendiği
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Kaydın durumu (Active, Passive, Deleted)
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

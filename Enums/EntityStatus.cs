namespace DigitalBanking.API.Enums;

// Veritabanındaki kaydın durumunu belirtir
public enum EntityStatus
{
    Active = 1,   // Kayıt aktif
    Passive = 2,  // Kayıt pasif
    Deleted = 3   // Soft Delete (Silinmiş ama DB'den kaldırılmamış)
}

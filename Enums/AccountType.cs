namespace DigitalBanking.API.Enums;

// Hesap türleri
public enum AccountType
{
    Checking = 1,    // Vadesiz (Günlük işlemler için)
    Savings = 2,     // Vadeli (Birikim için)
    Investment = 3   // Yatırım hesabı
}

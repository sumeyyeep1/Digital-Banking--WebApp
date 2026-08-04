using DigitalBanking.API.Enums;

namespace DigitalBanking.API.Models;

// Para transfer kayıtlarını tutar
public class Transaction : BaseEntity
{
    // Transfer türü (Yatırma, Çekme, Transfer)
    public TransactionType TransactionType { get; set; }

    // Transfer tutarı - decimal kullanıyoruz!
    public decimal Amount { get; set; }

    // Transfer açıklaması (opsiyonel)
    public string? Description { get; set; }

    // ----- İLİŞKİLER -----

    // Parayı gönderen hesabın ID'si (null olabilir: ATM'den yatırma gibi)
    public int? SenderAccountId { get; set; }

    // Parayı alan hesabın ID'si (null olabilir: ATM'den çekme gibi)
    public int? ReceiverAccountId { get; set; }

    // Navigation Property: Gönderen hesap
    // Restrict: Hesap silinse bile transfer geçmişi silinmesin!
    public virtual Account? SenderAccount { get; set; }

    // Navigation Property: Alan hesap
    public virtual Account? ReceiverAccount { get; set; }
}

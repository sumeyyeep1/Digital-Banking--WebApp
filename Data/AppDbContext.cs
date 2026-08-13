using DigitalBanking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.API.Data;

// Veritabanı bağlamı - EF Core'un veritabanıyla konuştuğu yer
public class AppDbContext : DbContext // Ef core hazır bir DbContext sınıfı sağlar, biz de bunu miras alıyoruz. Bu sayede veritabanı işlemlerini kolayca yapabiliriz.
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // options ile veritabanı bağlantı ayarlarını alır ve dbcontext sınıfına iletir. Bu sayede hangi veritabanına bağlanacağımızı belirleyebiliriz.
    {
    }

    // ===== TABLOLAR =====
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== BENZERSİZLİK KISITLAMALARI =====

        // Aynı email iki kullanıcıda olamaz
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Aynı TC Kimlik No iki müşteride olamaz
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.IdentityNumber)
            .IsUnique();

        // Aynı IBAN iki hesapta olamaz
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Iban)
            .IsUnique();

        modelBuilder.Entity<Card>()
            .ToTable("Cards");

        modelBuilder.Entity<Card>()
            .HasIndex(c => c.CardNumber)
            .IsUnique();

        // ===== PARA HASSASIYETI (decimal(18,2)) =====

        // Bakiye: 999.999.999.999.999,99 hassasiyetinde
        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);

        // Transfer tutarı aynı hassasiyette
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Card>()
            .Property(c => c.CardNumber)
            .HasMaxLength(16);

        modelBuilder.Entity<Card>()
            .Property(c => c.CardHolderName)
            .HasMaxLength(100);

        modelBuilder.Entity<Card>()
            .Property(c => c.ExpiryMonth)
            .HasMaxLength(2);

        modelBuilder.Entity<Card>()
            .Property(c => c.ExpiryYear)
            .HasMaxLength(4);

        // ===== TRANSFER İLİŞKİLERİ =====

        // Gönderen hesap silinse bile transfer kaydı silinmesin (Restrict)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.SenderAccount)
            .WithMany(a => a.SentTransactions)
            .HasForeignKey(t => t.SenderAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Alan hesap silinse bile transfer kaydı silinmesin (Restrict)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ReceiverAccount)
            .WithMany(a => a.ReceivedTransactions)
            .HasForeignKey(t => t.ReceiverAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Card>()
            .HasOne(c => c.Account)
            .WithMany(a => a.Cards)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

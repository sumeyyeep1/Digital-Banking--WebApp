using DigitalBanking.API.Data;
using DigitalBanking.API.DTOs.Accounts;
using DigitalBanking.API.Enums;
using DigitalBanking.API.Interfaces;
using DigitalBanking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.API.Services;
//İnterface den miras alır.
public class AccountService : IAccountService

{
    private readonly AppDbContext _context; //AppDbContext sınıfı db işlemleri için kullanılır.
    //Constructor injection ile AppDbContext'i alır.
    public AccountService(AppDbContext context)  //Context için constructor injection ile alınır.
    {
        _context = context;// Burada _context değişkeni, AppDbContext sınıfından üretildiği için o classtaki tüm metot ve propertylere erişir. ve veritabanı işlemlerini gerçekleştirmek için kullanılır.
    }
    //
    public async Task<List<AccountResponseDto>> GetMyAccountsAsync(int userId)// Task bu işlemin zaman alabileceğini belirtir ve liste şeklinde bir dto döndürür.
    {
        return await _context.Accounts// Accounts tablosundan _context üzerinden verileri çeker.
            .Where(a => a.Customer.UserId == userId && a.Status == EntityStatus.Active) // where metoduyla filtreleme yapılıyor.a her bir hesap kaydını temsil eder.Hesabın bağlı olduğu userıd değeri ile verilen userıd değeri eşleşiyorsa ve hesap durumu aktiifse buna göre filtreleme yapılır.
            .Select(a => new AccountResponseDto
            {
                Id = a.Id,
                Iban = a.Iban,
                AccountType = a.AccountType.ToString(),
                Currency = a.Currency.ToString(),
                Balance = a.Balance
            })
            .ToListAsync();
    }
    //Filtrelenen her hesap yeni bir accountResponseDto nesnesine dönüştürülür. Son olarak, ToListAsync() metodu ile bu liste asenkron olarak döndürülür.
    public async Task<AccountResponseDto?> CreateAccountAsync(int userId, CreateAccountRequestDto request) //Hesap ooluşturma metodu. Kullanıcı id ve createAccountRequestDto nesnesi alır. 
    {
        var customer = await _context.Customers //customers tablosundan verileri çeker.
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == EntityStatus.Active); //FirstOrDefaultAsync metodu, verilen koşula uyan ilk müşteri kaydını getirir. Eğer böyle bir kayıt yoksa null döner. Burada, müşteri kaydının userId'si verilen userId ile eşleşmeli ve müşteri durumu aktif olmalıdır.

        if (customer == null)
        {
            return null;
        }

        var account = new Account //Yeni bir account nesnesi oluşturulur.
        {
            CustomerId = customer.Id,
            AccountType = request.AccountType,
            Currency = request.Currency,
            Iban = await GenerateUniqueIbanAsync()
        };

        _context.Accounts.Add(account); //Yeni oluşturulan account nesnesi veritabanına eklenir.
        await _context.SaveChangesAsync(); // Değişiklikler veritabanına kaydedilir.

        return MapToResponse(account); // MapToResponse metodu ile account nesnesi AccountResponseDto nesnesine dönüştürülür ve döndürülür.
    }

    public async Task<AccountResponseDto?> UpdateAccountAsync(int userId, int accountId, UpdateAccountRequestDto request) //Hesap güncelleme metodu. İsteği yapan kullanıcı id, güncellenecek hesap id ve güncel bilgiler olarak da request nesnesi alır.
    {
        var account = await _context.Accounts //Accounts tablosundan verileri çeker.
            .Include(a => a.Customer) // Include metodu, ilişkili Customer verilerini de yükler. Bu sayede account nesnesi ile birlikte müşteri bilgilerine de erişilebilir.
            .FirstOrDefaultAsync(a => a.Id == accountId && a.Customer.UserId == userId && a.Status == EntityStatus.Active); // Verilen ıd  incelenen id ile aynı mı ,hesabın bağlı olduğu müşterinin userId'si verilen userId ile eşleşiyor mu ve hesabın durumu aktif mi kontrol edilir.
                                                                                                                            // Eğer bu koşulları sağlayan bir hesap bulunursa, account değişkenine atanır.
                                                                                                                            // Aksi takdirde null döner.
        if (account == null)
        {
            return null;
        }

        account.AccountType = request.AccountType;  // hesap türü request nesnesinden alınan yeni hesap türü ile güncellenir.
        account.UpdatedAt = DateTime.UtcNow; // Hesap güncellendiği zaman UpdatedAt property'si güncellenir.

        await _context.SaveChangesAsync(); // 

        return MapToResponse(account); // Güncellenen account nesnesi AccountResponseDto nesnesine dönüştürülür ve döndürülür.
    }

    private async Task<string> GenerateUniqueIbanAsync() // Yeni bir IBAN oluşturur ve veritabanında benzersiz olmasını sağlar.
    {
        string iban;

        do
        {
            iban = $"TR{Random.Shared.NextInt64(10_000_000_000_000_000, 99_999_999_999_999_999)}"; // Random.Shared.NextInt64 metodu, 10^16 ile 10^17 arasında rastgele bir sayı üretir ve TR ile birleştirerek IBAN oluşturur.
        }
        while (await _context.Accounts.AnyAsync(a => a.Iban == iban)); // AnyAsync metodu, veritabanında bu IBAN ile eşleşen herhangi bir hesap olup olmadığını kontrol eder.
                                                                       // Eğer böyle bir hesap varsa, döngü tekrar çalışır ve yeni bir IBAN üretilir.

        return iban;
    }

    private static AccountResponseDto MapToResponse(Account account)  // Account nesnesini AccountResponseDto nesnesine dönüştürür.
    { 
        return new AccountResponseDto // AccountResponseDto nesnesi oluşturulur ve account nesnesindeki verilerle doldurulur. 
        {
            Id = account.Id, 
            Iban = account.Iban,
            AccountType = account.AccountType.ToString(),
            Currency = account.Currency.ToString(),
            Balance = account.Balance
        };
    }
}

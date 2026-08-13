using System.Text;
using DigitalBanking.API.Data;
using DigitalBanking.API.Interfaces;
using DigitalBanking.API.Models;
using DigitalBanking.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args); // 

// =============================================
// 1. VERİTABANI BAĞLANTISI
// =============================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // DI: AppDbContext'i ihtiyac olan siniflara hazir olarak verir.

// =============================================
// 2. SERVİS KAYITLARI (Dependency Injection)
// =============================================
builder.Services.AddScoped<IAuthService, AuthService>(); // DI sayesinde bu servisler ihtiyac olan siniflara hazir olarak verilir.
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IMarketService, MarketService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>(); // DI: Sifre hashleme servisini ihtiyac olan siniflara hazir olarak verir.
builder.Services.AddHttpClient(); 
builder.Services.AddMemoryCache();
// =============================================
// 3. JWT AUTHENTICATION AYARLARI
// appsettings.json'daki JwtSettings'i buraya bağlıyoruz
// =============================================
var secretKey = builder.Configuration["JwtSettings:SecretKey"]!; 
var issuer = builder.Configuration["JwtSettings:Issuer"]!;
var audience = builder.Configuration["JwtSettings:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    // Varsayılan kimlik doğrulama yöntemi JWT olsun
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Gelen Token'ı nasıl doğrulayacağımızı tanımlıyoruz
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Token'ı kimin oluşturduğunu kontrol et
        ValidateIssuer = true,
        ValidIssuer = issuer,

        // Token'ın kime yönelik olduğunu kontrol et
        ValidateAudience = true,
        ValidAudience = audience,

        // Token'ın süresi dolmuş mu kontrol et
        ValidateLifetime = true,

        // Token'ın imzasını doğrula (değiştirilmemiş mi?)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// =============================================
// 4. YETKİLENDİRME (Authorization)
// [Authorize] attribute'u için gerekli
// =============================================
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost:3000", "http://127.0.0.1:3000") // yalnızca bu adreslerden gelen istekleri backend kabul eder
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =============================================
// 5. SWAGGER & CONTROLLER
// =============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Swagger'a JWT desteği ekle → "Authorize" butonu çıksın
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Token'ı şu formatta gir: Bearer {token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build(); // 

// =============================================
// 6. MIDDLEWARE SIRALAMA (Sıra önemli!)
// =============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Banking API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();// Https yönlendirmesi yapar. Yani http ile gelen istekleri otomatik olarak https'e yönlendirir.

app.UseCors("Frontend"); // CORS middleware'i ekler ve "Frontend" isimli CORS politikasını uygular. Bu sayede sadece belirlenen frontend adreslerinden gelen istekler kabul edilir.

// Önce kimlik doğrulama (Authentication): "Sen kimsin?"
app.UseAuthentication(); // 

// Sonra yetkilendirme (Authorization): "Buna iznin var mı?"
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();// WEB api başlatır ve gelen istekleri kontrol eder.

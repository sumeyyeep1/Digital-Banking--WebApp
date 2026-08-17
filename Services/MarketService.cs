using DigitalBanking.API.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DigitalBanking.API.Services;

public class MarketService : IMarketService
{
    private static readonly SemaphoreSlim CollectApiGate = new(1, 1); 
    private static DateTimeOffset _lastCollectApiRequest = DateTimeOffset.MinValue;

    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;

    public MarketService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _memoryCache = memoryCache;
    }

    public Task<MarketApiResponse> GetGoldPricesAsync()
    {
        return GetCollectApiResponseAsync("/economy/goldPrice");
    }

    public Task<MarketApiResponse> GetCurrencyRatesAsync()
    {
        return GetCollectApiResponseAsync("/economy/allCurrency");
    }

    public Task<MarketApiResponse> GetBistValuesAsync()
    {
        return GetCollectApiResponseAsync("/economy/borsaIstanbul");
    }

    public Task<MarketApiResponse> GetStocksAsync()
    {
        return GetCollectApiResponseAsync("/economy/hisseSenedi");
    }

    private async Task<MarketApiResponse> GetCollectApiResponseAsync(string path)
    {
        var cacheKey = $"collectapi:{path}";
        if (_memoryCache.TryGetValue(cacheKey, out MarketApiResponse? cachedResponse) && cachedResponse is not null)
        {
            return cachedResponse;
        }

        var apiKey = _configuration["CollectApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("COLLECT_API_KEY");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return ErrorResponse("CollectAPI anahtari tanimli degil. CollectApi__ApiKey veya COLLECT_API_KEY environment variable ile girin.", StatusCodes.Status400BadRequest);
        }

        var authorizationValue = apiKey.StartsWith("apikey ", StringComparison.OrdinalIgnoreCase)
            ? apiKey
            : $"apikey {apiKey}";

        await CollectApiGate.WaitAsync();
        try
        {
            if (_memoryCache.TryGetValue(cacheKey, out cachedResponse) && cachedResponse is not null)
            {
                return cachedResponse;
            }

            var elapsed = DateTimeOffset.UtcNow - _lastCollectApiRequest;
            if (elapsed < TimeSpan.FromSeconds(1.1))
            {
                await Task.Delay(TimeSpan.FromSeconds(1.1) - elapsed);
            }

            var baseUrl = _configuration["CollectApi:BaseUrl"] ?? "https://api.collectapi.com";
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_configuration.GetValue("CollectApi:TimeoutSeconds", 10));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl.TrimEnd('/')}{path}");
            request.Headers.TryAddWithoutValidation("Authorization", authorizationValue);

            using var response = await client.SendAsync(request);
            _lastCollectApiRequest = DateTimeOffset.UtcNow;

            var responseBody = await response.Content.ReadAsStringAsync();
            var marketResponse = new MarketApiResponse(responseBody, (int)response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var cacheMinutes = _configuration.GetValue("CollectApi:CacheMinutes", 5);
                _memoryCache.Set(cacheKey, marketResponse, TimeSpan.FromMinutes(cacheMinutes));
            }

            return marketResponse;
        }
        catch (TaskCanceledException)
        {
            return ErrorResponse("CollectAPI yaniti zaman asimina ugradi. Biraz sonra tekrar deneyin.", StatusCodes.Status504GatewayTimeout);
        }
        catch (HttpRequestException)
        {
            return ErrorResponse("CollectAPI baglantisi basarisiz oldu. Internet baglantisi veya API erisimi kontrol edilmeli.", StatusCodes.Status502BadGateway);
        }
        finally
        {
            CollectApiGate.Release();
        }
    }

    private static MarketApiResponse ErrorResponse(string message, int statusCode)
    {
        return new MarketApiResponse($$"""{"success":false,"message":"{{message}}"}""", statusCode);
    }
}

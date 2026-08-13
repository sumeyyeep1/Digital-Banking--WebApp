using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DigitalBanking.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private static readonly SemaphoreSlim CollectApiGate = new(1, 1);
    private static DateTimeOffset _lastCollectApiRequest = DateTimeOffset.MinValue;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;

    public MarketController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _memoryCache = memoryCache;
    }

    [HttpGet("gold")]
    public Task<IActionResult> GetGoldPrices()
    {
        return GetCollectApiResponseAsync("/economy/goldPrice");
    }

    [HttpGet("currency")]
    public Task<IActionResult> GetCurrencyRates()
    {
        return GetCollectApiResponseAsync("/economy/allCurrency");
    }

    [HttpGet("bist")]
    public Task<IActionResult> GetBistValues()
    {
        return GetCollectApiResponseAsync("/economy/borsaIstanbul");
    }

    [HttpGet("stocks")]
    public Task<IActionResult> GetStocks()
    {
        return GetCollectApiResponseAsync("/economy/hisseSenedi");
    }

    private async Task<IActionResult> GetCollectApiResponseAsync(string path)
    {
        var cacheKey = $"collectapi:{path}";
        if (_memoryCache.TryGetValue(cacheKey, out CachedMarketResponse? cachedResponse) && cachedResponse is not null)
        {
            return CreateJsonResult(cachedResponse);
        }

        var apiKey = _configuration["CollectApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return BadRequest(new { message = "CollectAPI token tanimli degil. appsettings.Development.json veya environment variable ile CollectApi__ApiKey girin." });
        }

        var authorizationValue = apiKey.StartsWith("apikey ", StringComparison.OrdinalIgnoreCase)
            ? apiKey
            : $"apikey {apiKey}";

        var baseUrl = _configuration["CollectApi:BaseUrl"] ?? "https://api.collectapi.com";
        var client = _httpClientFactory.CreateClient();

        await CollectApiGate.WaitAsync();
        try
        {
            if (_memoryCache.TryGetValue(cacheKey, out cachedResponse) && cachedResponse is not null)
            {
                return CreateJsonResult(cachedResponse);
            }

            var elapsed = DateTimeOffset.UtcNow - _lastCollectApiRequest;
            if (elapsed < TimeSpan.FromSeconds(1.1))
            {
                await Task.Delay(TimeSpan.FromSeconds(1.1) - elapsed);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl.TrimEnd('/')}{path}");
            request.Headers.TryAddWithoutValidation("Authorization", authorizationValue);

            using var response = await client.SendAsync(request);
            _lastCollectApiRequest = DateTimeOffset.UtcNow;

            var responseBody = await response.Content.ReadAsStringAsync();
            var marketResponse = new CachedMarketResponse(responseBody, (int)response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var cacheMinutes = _configuration.GetValue("CollectApi:CacheMinutes", 5);
                _memoryCache.Set(cacheKey, marketResponse, TimeSpan.FromMinutes(cacheMinutes));
            }

            return CreateJsonResult(marketResponse);
        }
        finally
        {
            CollectApiGate.Release();
        }
    }

    private static ContentResult CreateJsonResult(CachedMarketResponse response)
    {
        return new ContentResult
        {
            Content = response.Content,
            ContentType = "application/json; charset=utf-8",
            StatusCode = response.StatusCode
        };
    }

    private sealed record CachedMarketResponse(string Content, int StatusCode);
}

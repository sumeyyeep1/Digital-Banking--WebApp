namespace DigitalBanking.API.Interfaces;

public interface IMarketService
{
    Task<MarketApiResponse> GetGoldPricesAsync();
    Task<MarketApiResponse> GetCurrencyRatesAsync();
    Task<MarketApiResponse> GetBistValuesAsync();
    Task<MarketApiResponse> GetStocksAsync();
}

public sealed record MarketApiResponse(string Content, int StatusCode);

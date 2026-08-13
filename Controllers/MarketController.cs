using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DigitalBanking.API.Interfaces;

namespace DigitalBanking.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly IMarketService _marketService;

    public MarketController(IMarketService marketService)
    {
        _marketService = marketService;
    }

    [HttpGet("gold")]
    public async Task<IActionResult> GetGoldPrices()
    {
        return CreateJsonResult(await _marketService.GetGoldPricesAsync());
    }

    [HttpGet("currency")]
    public async Task<IActionResult> GetCurrencyRates()
    {
        return CreateJsonResult(await _marketService.GetCurrencyRatesAsync());
    }

    [HttpGet("bist")]
    public async Task<IActionResult> GetBistValues()
    {
        return CreateJsonResult(await _marketService.GetBistValuesAsync());
    }

    [HttpGet("stocks")]
    public async Task<IActionResult> GetStocks()
    {
        return CreateJsonResult(await _marketService.GetStocksAsync());
    }

    private static ContentResult CreateJsonResult(MarketApiResponse response)
    {
        return new ContentResult
        {
            Content = response.Content,
            ContentType = "application/json; charset=utf-8",
            StatusCode = response.StatusCode
        };
    }
}

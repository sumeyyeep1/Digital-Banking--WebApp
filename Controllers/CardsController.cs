using System.Security.Claims;
using DigitalBanking.API.DTOs.Cards;
using DigitalBanking.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBanking.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCards()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var cards = await _cardService.GetMyCardsAsync(userId.Value);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cardService.CreateCardAsync(userId.Value, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{cardId:int}")]
    public async Task<IActionResult> UpdateCard(int cardId, [FromBody] UpdateCardRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cardService.UpdateCardAsync(userId.Value, cardId, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

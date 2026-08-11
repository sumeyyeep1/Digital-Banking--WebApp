using System.Security.Claims;
using DigitalBanking.API.DTOs.Cards;
using DigitalBanking.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBanking.API.Controllers;
 
[ApiController] 
[Authorize] // Bu uygulamadaki endpointler için yetkilendirme gerektiðini söylüyor.
[Route("api/[controller]")] // controllerýn adresi /api/cards olur.
public class CardsController : ControllerBase 
{
    private readonly ICardService _cardService; 

    public CardsController(ICardService cardService) // Constructor: Controller olusurken ihtiyac duydugu servisi alir.
                                                     // cardservice dý dan gelen nesne _cardservice ise o nesneyi saklayan field olur.
    {
        _cardService = cardService;
    }

    [HttpGet("my")] //metodun get isteði alacaðýný ve adresin /api/cards/my olacaðýný belirtir
    public async Task<IActionResult> GetMyCards() 
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(); // yetki yok 401 döner.
        }

        var cards = await _cardService.GetMyCardsAsync(userId.Value); //
        return Ok(cards);
    }

    [HttpPost] //metodun post isteði alacaðýný belirtir.
    public async Task<IActionResult> CreateCard([FromBody] CreateCardRequestDto request) // frombody ile json belirtilen dtoya dönüþtürülür.
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cardService.CreateCardAsync(userId.Value, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result); // result baþarýlý ise 200 döner, deðilse 400 döner.
    }

    [HttpPut("{cardId:int}")] // metodun put isteði alacaðýný ve adresin /api/cards/{cardId} olacaðýný belirtir. cardId int tipinde olmalý.
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

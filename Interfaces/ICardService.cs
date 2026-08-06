using DigitalBanking.API.DTOs.Cards;

namespace DigitalBanking.API.Interfaces;

public interface ICardService
{
    Task<List<CardResponseDto>> GetMyCardsAsync(int userId);
    Task<CardOperationResponseDto> CreateCardAsync(int userId, CreateCardRequestDto request);
    Task<CardOperationResponseDto> UpdateCardAsync(int userId, int cardId, UpdateCardRequestDto request);
}

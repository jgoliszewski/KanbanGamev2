using Microsoft.AspNetCore.Mvc;
using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameStateController : ControllerBase
{
    private readonly IGameStateService _gameStateService;

    public GameStateController(IGameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    [HttpGet]
    public IActionResult GetGameState()
    {
        return Ok(new
        {
            CurrentDay = _gameStateService.CurrentDay,
            GameStartDate = _gameStateService.GameStartDate,
            UnlockedAchievements = _gameStateService.UnlockedAchievements
        });
    }

    [HttpPost("nextday")]
    public async Task<IActionResult> AdvanceToNextDay()
    {
        await _gameStateService.AdvanceToNextDay();
        return Ok(new { CurrentDay = _gameStateService.CurrentDay });
    }

    [HttpPost("achievement")]
    public async Task<IActionResult> UnlockAchievement([FromBody] Achievement achievement)
    {
        await _gameStateService.UnlockAchievement(achievement);
        return Ok();
    }
} 
using Microsoft.AspNetCore.Mvc;
using KanbanGamev2.Shared.Services;
using KanbanGamev2.Server.Services;

namespace KanbanGamev2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameStateController : ControllerBase
{
    private readonly IGameStateService _gameStateService;
    private readonly IGameRestartService _gameRestartService;

    public GameStateController(IGameStateService gameStateService, IGameRestartService gameRestartService)
    {
        _gameStateService = gameStateService;
        _gameRestartService = gameRestartService;
    }

    [HttpGet]
    public IActionResult GetGameState()
    {
        return Ok(new
        {
            CurrentDay = _gameStateService.CurrentDay,
            GameStartDate = _gameStateService.GameStartDate,
            UnlockedAchievements = _gameStateService.UnlockedAchievements,
            CompanyMoney = _gameStateService.CompanyMoney,
            MoneyTransactions = _gameStateService.MoneyTransactions,
            IsSummaryBoardVisible = _gameStateService.IsSummaryBoardVisible,
            IsReadyForDevelopmentColumnVisible = _gameStateService.IsReadyForDevelopmentColumnVisible
        });
    }

    [HttpPost("nextday")]
    public async Task<IActionResult> AdvanceToNextDay()
    {
        await _gameStateService.AdvanceToNextDay();
        return Ok(new { CurrentDay = _gameStateService.CurrentDay });
    }

    [HttpPost("addmoney/{amount}")]
    public async Task<IActionResult> AddMoney(decimal amount)
    {
        await _gameStateService.AddMoney(amount);
        return Ok(new { CompanyMoney = _gameStateService.CompanyMoney });
    }

    [HttpPost("setmoney/{amount}")]
    public async Task<IActionResult> SetMoney(decimal amount)
    {
        await _gameStateService.SetMoney(amount);
        return Ok(new { CompanyMoney = _gameStateService.CompanyMoney });
    }

    [HttpPost("achievement")]
    public async Task<IActionResult> UnlockAchievement([FromBody] Achievement achievement)
    {
        await _gameStateService.UnlockAchievement(achievement);
        return Ok();
    }

    [HttpPost("summary-board-visibility")]
    public async Task<IActionResult> SetSummaryBoardVisibility([FromBody] bool isVisible)
    {
        await _gameStateService.SetSummaryBoardVisibility(isVisible);
        return Ok(new { IsSummaryBoardVisible = _gameStateService.IsSummaryBoardVisible });
    }

    [HttpPost("ready-dev-column-visibility")]
    public async Task<IActionResult> SetReadyForDevelopmentColumnVisibility([FromBody] bool isVisible)
    {
        await _gameStateService.SetReadyForDevelopmentColumnVisibility(isVisible);
        return Ok(new { IsReadyForDevelopmentColumnVisible = _gameStateService.IsReadyForDevelopmentColumnVisible });
    }

    [HttpPost("restart")]
    public async Task<IActionResult> RestartGame()
    {
        await _gameRestartService.RestartGameAsync();
        return Ok();
    }
} 
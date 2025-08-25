using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using KanbanGame.Shared;
using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Client.Services;

public class GameStateService : IGameStateService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IGameStateManager _gameStateManager;
    private readonly ISignalRService _signalRService;

    public int CurrentDay => _gameStateManager.CurrentDay;
    public DateTime GameStartDate => _gameStateManager.GameStartDate;
    public List<Achievement> UnlockedAchievements => _gameStateManager.UnlockedAchievements.ToList();
    public decimal CompanyMoney => _gameStateManager.CompanyMoney;
    public List<MoneyTransaction> MoneyTransactions => _gameStateManager.MoneyTransactions.ToList();
    public bool IsSummaryBoardVisible => _gameStateManager.IsSummaryBoardVisible;

    public event Action<int>? DayChanged
    {
        add => _gameStateManager.DayChanged += value;
        remove => _gameStateManager.DayChanged -= value;
    }
    
    public event Action<Achievement>? AchievementUnlocked
    {
        add => _gameStateManager.AchievementUnlocked += value;
        remove => _gameStateManager.AchievementUnlocked -= value;
    }

    public event Action<decimal>? MoneyChanged
    {
        add => _gameStateManager.MoneyChanged += value;
        remove => _gameStateManager.MoneyChanged -= value;
    }

    public event Action<bool>? SummaryBoardVisibilityChanged
    {
        add => _gameStateManager.SummaryBoardVisibilityChanged += value;
        remove => _gameStateManager.SummaryBoardVisibilityChanged -= value;
    }

    public GameStateService(HttpClient httpClient, IGameStateManager gameStateManager, ISignalRService signalRService)
    {
        _httpClient = httpClient;
        _gameStateManager = gameStateManager;
        _signalRService = signalRService;
        
        // Subscribe to SignalR events for real-time updates
        _signalRService.SummaryBoardVisibilityChangedFromServer += OnSummaryBoardVisibilityChangedFromServer;
    }

    public async Task AdvanceToNextDay()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/gamestate/nextday", null);
            if (response.IsSuccessStatusCode)
            {
                // Get the updated game state from the server
                await LoadGameState();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to advance to next day: {ex.Message}");
        }
    }

    public async Task LoadGameState()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/gamestate");
            if (response.IsSuccessStatusCode)
            {
                var gameState = await response.Content.ReadFromJsonAsync<GameStateResponse>();
                if (gameState != null)
                {
                    _gameStateManager.UpdateFromServer(
                        gameState.CurrentDay, 
                        gameState.GameStartDate, 
                        gameState.UnlockedAchievements,
                        gameState.CompanyMoney,
                        gameState.MoneyTransactions,
                        gameState.IsSummaryBoardVisible
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game state: {ex.Message}");
        }
    }

    public async Task SetSummaryBoardVisibility(bool isVisible)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/gamestate/summary-board-visibility", isVisible);
            if (response.IsSuccessStatusCode)
            {
                // Update local state
                _gameStateManager.SetSummaryBoardVisibility(isVisible);
            }
            else
            {
                Console.WriteLine($"Failed to set summary board visibility: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set summary board visibility: {ex.Message}");
        }
    }

    public void SetSummaryBoardVisibilityFromServer(bool isVisible)
    {
        // This method is called when the server sends a SignalR notification
        // about a summary board visibility change from another client
        _gameStateManager.SetSummaryBoardVisibility(isVisible);
    }

    public async Task AddMoney(decimal amount, string description = "Feature completed")
    {
        try
        {
            // Update local state first
            _gameStateManager.AddMoney(amount);
            
            // Persist to server
            var response = await _httpClient.PostAsync($"api/gamestate/addmoney/{amount}", null);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to persist money change to server: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add money: {ex.Message}");
        }
    }

    public async Task SetMoney(decimal amount)
    {
        _gameStateManager.SetMoney(amount);
        await Task.CompletedTask;
    }

    private class GameStateResponse
    {
        public int CurrentDay { get; set; }
        public DateTime GameStartDate { get; set; }
        public List<Achievement> UnlockedAchievements { get; set; } = new();
        public decimal CompanyMoney { get; set; }
        public List<MoneyTransaction> MoneyTransactions { get; set; } = new();
        public bool IsSummaryBoardVisible { get; set; }
    }

    public async Task UnlockAchievement(Achievement achievement)
    {
        _gameStateManager.NotifyAchievementUnlocked(achievement);
        await Task.CompletedTask;
    }

    public async Task<bool> IsAchievementUnlocked(string achievementId)
    {
        return await Task.FromResult(_gameStateManager.UnlockedAchievements.Any(a => a.Id == achievementId));
    }

    public async Task RestartGame()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/gamestate/restart", null);
            if (response.IsSuccessStatusCode)
            {
                // Reload the game state after restart
                await LoadGameState();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to restart game: {ex.Message}");
        }
    }

    private void OnSummaryBoardVisibilityChangedFromServer(bool isVisible)
    {
        _gameStateManager.SetSummaryBoardVisibility(isVisible);
    }

    public void Dispose()
    {
        _signalRService.SummaryBoardVisibilityChangedFromServer -= OnSummaryBoardVisibilityChangedFromServer;
    }
} 
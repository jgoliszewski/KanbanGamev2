using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using KanbanGame.Shared;
using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Client.Services;

public class GameStateService : IGameStateService
{
    private readonly HttpClient _httpClient;
    private readonly IGameStateManager _gameStateManager;

    public int CurrentDay => _gameStateManager.CurrentDay;
    public DateTime GameStartDate => _gameStateManager.GameStartDate;
    public List<Achievement> UnlockedAchievements => _gameStateManager.UnlockedAchievements.ToList();
    public decimal CompanyMoney => _gameStateManager.CompanyMoney;
    public List<MoneyTransaction> MoneyTransactions => _gameStateManager.MoneyTransactions.ToList();

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

    public GameStateService(HttpClient httpClient, IGameStateManager gameStateManager)
    {
        _httpClient = httpClient;
        _gameStateManager = gameStateManager;
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
                        gameState.MoneyTransactions
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game state: {ex.Message}");
        }
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
} 
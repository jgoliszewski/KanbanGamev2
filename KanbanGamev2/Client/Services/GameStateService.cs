using KanbanGamev2.Shared.Services;
using System.Text.Json;

namespace KanbanGamev2.Client.Services;

public class GameStateService : IGameStateService
{
    private readonly HttpClient _httpClient;
    private int _currentDay = 1;
    private DateTime _gameStartDate = DateTime.Now;
    private readonly List<Achievement> _unlockedAchievements = new();

    public int CurrentDay => _currentDay;
    public DateTime GameStartDate => _gameStartDate;
    public List<Achievement> UnlockedAchievements => _unlockedAchievements.ToList();

    public event Action<int>? DayChanged;
    public event Action<Achievement>? AchievementUnlocked;

    public GameStateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AdvanceToNextDay()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/gamestate/nextday", null);
            if (response.IsSuccessStatusCode)
            {
                _currentDay++;
                DayChanged?.Invoke(_currentDay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to advance to next day: {ex.Message}");
        }
    }

    public async Task UnlockAchievement(Achievement achievement)
    {
        if (!_unlockedAchievements.Any(a => a.Id == achievement.Id))
        {
            _unlockedAchievements.Add(achievement);
            AchievementUnlocked?.Invoke(achievement);
        }
        
        await Task.CompletedTask;
    }

    public async Task<bool> IsAchievementUnlocked(string achievementId)
    {
        return await Task.FromResult(_unlockedAchievements.Any(a => a.Id == achievementId));
    }
} 
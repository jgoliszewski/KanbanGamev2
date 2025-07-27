using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Server.Services;

public class GameStateService : IGameStateService
{
    private int _currentDay = 1;
    private readonly DateTime _gameStartDate = DateTime.Now;
    private readonly List<Achievement> _unlockedAchievements = new();

    public int CurrentDay => _currentDay;
    public DateTime GameStartDate => _gameStartDate;
    public List<Achievement> UnlockedAchievements => _unlockedAchievements.ToList();

    public event Action<int>? DayChanged;
    public event Action<Achievement>? AchievementUnlocked;

    public async Task AdvanceToNextDay()
    {
        _currentDay++;
        DayChanged?.Invoke(_currentDay);
        
        // Check for daily achievements
        await CheckDailyAchievements();
        
        await Task.CompletedTask;
    }

    public async Task LoadGameState()
    {
        // This method is mainly for client-side use
        // The server already has the current state
        await Task.CompletedTask;
    }

    public async Task UnlockAchievement(Achievement achievement)
    {
        if (!_unlockedAchievements.Any(a => a.Id == achievement.Id))
        {
            achievement.UnlockedAt = DateTime.Now;
            _unlockedAchievements.Add(achievement);
            AchievementUnlocked?.Invoke(achievement);
        }
        
        await Task.CompletedTask;
    }

    public async Task<bool> IsAchievementUnlocked(string achievementId)
    {
        return await Task.FromResult(_unlockedAchievements.Any(a => a.Id == achievementId));
    }

    private async Task CheckDailyAchievements()
    {
        // Example daily achievement for completing a day
        var dailyAchievement = new Achievement
        {
            Id = $"daily_complete_{_currentDay}",
            Name = $"Day {_currentDay} Complete",
            Description = $"Successfully completed day {_currentDay}",
            Icon = "📅",
            Type = AchievementType.Daily
        };

        await UnlockAchievement(dailyAchievement);
    }
} 
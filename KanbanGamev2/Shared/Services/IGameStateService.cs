namespace KanbanGamev2.Shared.Services;

public interface IGameStateService
{
    int CurrentDay { get; }
    DateTime GameStartDate { get; }
    List<Achievement> UnlockedAchievements { get; }
    decimal CompanyMoney { get; }
    
    event Action<int>? DayChanged;
    event Action<Achievement>? AchievementUnlocked;
    event Action<decimal>? MoneyChanged;
    
    Task AdvanceToNextDay();
    Task LoadGameState();
    Task UnlockAchievement(Achievement achievement);
    Task<bool> IsAchievementUnlocked(string achievementId);
    Task AddMoney(decimal amount);
    Task SetMoney(decimal amount);
}

public class Achievement
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }
    public AchievementType Type { get; set; }
}

public enum AchievementType
{
    Daily,
    Milestone,
    Special
} 
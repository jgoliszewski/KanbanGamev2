using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Client.Services;

public interface IGameStateManager
{
    int CurrentDay { get; set; }
    DateTime GameStartDate { get; set; }
    List<Achievement> UnlockedAchievements { get; set; }
    
    event Action<int>? DayChanged;
    event Action<Achievement>? AchievementUnlocked;
    
    void UpdateFromServer(int currentDay, DateTime gameStartDate, List<Achievement> achievements);
    void NotifyDayChanged(int newDay);
    void NotifyAchievementUnlocked(Achievement achievement);
}

public class GameStateManager : IGameStateManager
{
    private int _currentDay = 1;
    private DateTime _gameStartDate = DateTime.Now;
    private List<Achievement> _unlockedAchievements = new();

    public int CurrentDay 
    { 
        get => _currentDay; 
        set => _currentDay = value; 
    }
    
    public DateTime GameStartDate 
    { 
        get => _gameStartDate; 
        set => _gameStartDate = value; 
    }
    
    public List<Achievement> UnlockedAchievements 
    { 
        get => _unlockedAchievements; 
        set => _unlockedAchievements = value; 
    }

    public event Action<int>? DayChanged;
    public event Action<Achievement>? AchievementUnlocked;

    public void UpdateFromServer(int currentDay, DateTime gameStartDate, List<Achievement> achievements)
    {
        _currentDay = currentDay;
        _gameStartDate = gameStartDate;
        _unlockedAchievements = achievements ?? new List<Achievement>();
        DayChanged?.Invoke(_currentDay);
    }

    public void NotifyDayChanged(int newDay)
    {
        _currentDay = newDay;
        DayChanged?.Invoke(_currentDay);
    }

    public void NotifyAchievementUnlocked(Achievement achievement)
    {
        if (!_unlockedAchievements.Any(a => a.Id == achievement.Id))
        {
            _unlockedAchievements.Add(achievement);
            AchievementUnlocked?.Invoke(achievement);
        }
    }
} 
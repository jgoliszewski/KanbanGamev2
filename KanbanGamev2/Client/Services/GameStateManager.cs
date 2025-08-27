using KanbanGame.Shared;
using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Client.Services;

public interface IGameStateManager
{
    int CurrentDay { get; set; }
    DateTime GameStartDate { get; set; }
    List<Achievement> UnlockedAchievements { get; set; }
    decimal CompanyMoney { get; set; }
    List<MoneyTransaction> MoneyTransactions { get; set; }
    bool IsSummaryBoardVisible { get; set; }
    bool IsReadyForDevelopmentColumnVisible { get; set; }
    
    event Action<int>? DayChanged;
    event Action<Achievement>? AchievementUnlocked;
    event Action<decimal>? MoneyChanged;
    event Action<bool>? SummaryBoardVisibilityChanged;
    event Action<bool>? ReadyForDevelopmentColumnVisibilityChanged;
    
    void UpdateFromServer(int currentDay, DateTime gameStartDate, List<Achievement> achievements, decimal companyMoney, List<MoneyTransaction> moneyTransactions, bool isSummaryBoardVisible, bool isReadyForDevelopmentColumnVisible);
    void NotifyDayChanged(int newDay);
    void NotifyAchievementUnlocked(Achievement achievement);
    void AddMoney(decimal amount);
    void SetMoney(decimal amount);
    void SetSummaryBoardVisibility(bool isVisible);
    void SetReadyForDevelopmentColumnVisibility(bool isVisible);
}

public class GameStateManager : IGameStateManager
{
    private int _currentDay = 1;
    private DateTime _gameStartDate = DateTime.Now;
    private List<Achievement> _unlockedAchievements = new();
    private decimal _companyMoney = 10000;
    private List<MoneyTransaction> _moneyTransactions = new();
    private bool _isSummaryBoardVisible = false;
    private bool _isReadyForDevelopmentColumnVisible = false;

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

    public decimal CompanyMoney
    {
        get => _companyMoney;
        set => _companyMoney = value;
    }
    
    public List<MoneyTransaction> MoneyTransactions
    {
        get => _moneyTransactions;
        set => _moneyTransactions = value;
    }

    public bool IsSummaryBoardVisible
    {
        get => _isSummaryBoardVisible;
        set => _isSummaryBoardVisible = value;
    }

    public bool IsReadyForDevelopmentColumnVisible
    {
        get => _isReadyForDevelopmentColumnVisible;
        set => _isReadyForDevelopmentColumnVisible = value;
    }

    public event Action<int>? DayChanged;
    public event Action<Achievement>? AchievementUnlocked;
    public event Action<decimal>? MoneyChanged;
    public event Action<bool>? SummaryBoardVisibilityChanged;
    public event Action<bool>? ReadyForDevelopmentColumnVisibilityChanged;

    public void UpdateFromServer(int currentDay, DateTime gameStartDate, List<Achievement> achievements, decimal companyMoney, List<MoneyTransaction> moneyTransactions, bool isSummaryBoardVisible, bool isReadyForDevelopmentColumnVisible)
    {
        _currentDay = currentDay;
        _gameStartDate = gameStartDate;
        _unlockedAchievements = achievements ?? new List<Achievement>();
        _companyMoney = companyMoney;
        _moneyTransactions = moneyTransactions ?? new List<MoneyTransaction>();
        _isSummaryBoardVisible = isSummaryBoardVisible;
        _isReadyForDevelopmentColumnVisible = isReadyForDevelopmentColumnVisible;
        DayChanged?.Invoke(_currentDay);
        MoneyChanged?.Invoke(_companyMoney);
        SummaryBoardVisibilityChanged?.Invoke(_isSummaryBoardVisible);
        ReadyForDevelopmentColumnVisibilityChanged?.Invoke(_isReadyForDevelopmentColumnVisible);
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

    public void AddMoney(decimal amount)
    {
        _companyMoney += amount;
        MoneyChanged?.Invoke(_companyMoney);
    }

    public void SetMoney(decimal amount)
    {
        _companyMoney = amount;
        MoneyChanged?.Invoke(_companyMoney);
    }

    public void SetSummaryBoardVisibility(bool isVisible)
    {
        _isSummaryBoardVisible = isVisible;
        SummaryBoardVisibilityChanged?.Invoke(_isSummaryBoardVisible);
    }

    public void SetReadyForDevelopmentColumnVisibility(bool isVisible)
    {
        _isReadyForDevelopmentColumnVisible = isVisible;
        ReadyForDevelopmentColumnVisibilityChanged?.Invoke(_isReadyForDevelopmentColumnVisible);
    }
} 
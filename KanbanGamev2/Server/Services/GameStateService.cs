using KanbanGame.Shared;
using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Server.Services;

public class GameStateService : IGameStateService
{
    private int _currentDay = 1;
    private readonly DateTime _gameStartDate = DateTime.Now;
    private readonly List<Achievement> _unlockedAchievements = new();
    private decimal _companyMoney = 10000; // Starting money
    private readonly List<MoneyTransaction> _moneyTransactions = new();

    public GameStateService()
    {
        // Start with some progress already made
        _currentDay = 5;
        _companyMoney = 25000; // Starting with some money from completed work
        
        // Add some initial money transactions to simulate completed work
        _moneyTransactions.Add(new MoneyTransaction
        {
            Id = Guid.NewGuid(),
            Amount = 15000,
            Description = "User Authentication Feature Completed",
            Timestamp = DateTime.Now.AddDays(-3),
            Type = TransactionType.Income
        });
        
        _moneyTransactions.Add(new MoneyTransaction
        {
            Id = Guid.NewGuid(),
            Amount = 8000,
            Description = "Dashboard Analytics Feature Completed",
            Timestamp = DateTime.Now.AddDays(-2),
            Type = TransactionType.Income
        });
        
        _moneyTransactions.Add(new MoneyTransaction
        {
            Id = Guid.NewGuid(),
            Amount = -2000,
            Description = "Employee Salaries",
            Timestamp = DateTime.Now.AddDays(-1),
            Type = TransactionType.Expense
        });
    }

    public int CurrentDay => _currentDay;
    public DateTime GameStartDate => _gameStartDate;
    public List<Achievement> UnlockedAchievements => _unlockedAchievements.ToList();
    public decimal CompanyMoney => _companyMoney;
    public List<MoneyTransaction> MoneyTransactions => _moneyTransactions.ToList();

    public event Action<int>? DayChanged;
    public event Action<Achievement>? AchievementUnlocked;
    public event Action<decimal>? MoneyChanged;

    public async Task AdvanceToNextDay()
    {
        _currentDay++;
        DayChanged?.Invoke(_currentDay);
        
        // No more daily achievements - only feature completion achievements
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

    public async Task AddMoney(decimal amount, string description = "Feature completed")
    {
        _companyMoney += amount;
        
        // Record the transaction
        var transaction = new MoneyTransaction
        {
            Amount = amount,
            Description = description,
            Type = TransactionType.Income
        };
        _moneyTransactions.Add(transaction);
        
        MoneyChanged?.Invoke(_companyMoney);
        await Task.CompletedTask;
    }

    public async Task SetMoney(decimal amount)
    {
        _companyMoney = amount;
        MoneyChanged?.Invoke(_companyMoney);
        await Task.CompletedTask;
    }
} 
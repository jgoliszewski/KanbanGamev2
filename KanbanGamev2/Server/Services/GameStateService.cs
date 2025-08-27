using KanbanGame.Shared;
using KanbanGamev2.Server.SignalR;
using KanbanGamev2.Shared.Services;
using Microsoft.AspNetCore.SignalR;

namespace KanbanGamev2.Server.Services;

public class GameStateService : IGameStateService
{
    private int _currentDay = 1;
    private readonly DateTime _gameStartDate = DateTime.Now;
    private readonly List<Achievement> _unlockedAchievements = new();
    private decimal _companyMoney = 10000; // Starting money
    private readonly List<MoneyTransaction> _moneyTransactions = new();
    private readonly IEmployeeService _employeeService;
    private readonly IHubContext<GameHub> _gameHub;
    private bool _isSummaryBoardVisible = true; // Default to visible
    private bool _isReadyForDevelopmentColumnVisible = true; // Default to visible

    public GameStateService(IEmployeeService employeeService, IHubContext<GameHub> gameHub)
    {
        _employeeService = employeeService;
        _gameHub = gameHub;
        // Start with initial values
        _currentDay = 1;
        _companyMoney = 10000; // Starting money
        _isSummaryBoardVisible = true; // Default to visible
        _isReadyForDevelopmentColumnVisible = true; // Default to visible

        // No initial money transactions - start fresh
    }

    public int CurrentDay => _currentDay;
    public DateTime GameStartDate => _gameStartDate;
    public List<Achievement> UnlockedAchievements => _unlockedAchievements.ToList();
    public decimal CompanyMoney => _companyMoney;
    public List<MoneyTransaction> MoneyTransactions => _moneyTransactions.ToList();
    public bool IsSummaryBoardVisible => _isSummaryBoardVisible;
    public bool IsReadyForDevelopmentColumnVisible => _isReadyForDevelopmentColumnVisible;

    public event Action<int>? DayChanged;
    public event Action<Achievement>? AchievementUnlocked;
    public event Action<decimal>? MoneyChanged;
    public event Action<bool>? SummaryBoardVisibilityChanged;
    public event Action<bool>? ReadyForDevelopmentColumnVisibilityChanged;

    public async Task AdvanceToNextDay()
    {
        _currentDay++;
        DayChanged?.Invoke(_currentDay);

        // Process vacation days for all employees
        await ProcessVacationDays();

        // No more daily achievements - only feature completion achievements
        await Task.CompletedTask;
    }

    private async Task ProcessVacationDays()
    {
        try
        {
            Console.WriteLine($"Processing vacation days for day {_currentDay}...");
            Console.WriteLine($"Current real-world date: {DateTime.Today:yyyy-MM-dd}");

            // Get all employees currently on vacation
            var employees = _employeeService.GetEmployees();
            var employeesOnVacation = employees.Where(e => e.Status == EmployeeStatus.OnVacation && e.VacationEndDate.HasValue).ToList();

            Console.WriteLine($"Found {employeesOnVacation.Count} employees on vacation");

            foreach (var employee in employeesOnVacation)
            {
                if (employee.VacationEndDate.HasValue)
                {
                    var oldEndDate = employee.VacationEndDate.Value;

                    // Calculate remaining vacation days based on the current vacation end date
                    var daysRemaining = (oldEndDate.Date - DateTime.Today.Date).Days;

                    Console.WriteLine($"Employee {employee.Name}: Vacation end date: {oldEndDate.Date:yyyy-MM-dd}, Days remaining: {daysRemaining}");

                    // Decrement by 1 day since we're advancing to the next day
                    var newDaysRemaining = daysRemaining - 1;
                    Console.WriteLine($"Employee {employee.Name}: After decrement: {newDaysRemaining} days remaining");

                    if (newDaysRemaining <= 0)
                    {
                        Console.WriteLine($"Employee {employee.Name} vacation ended, returning to work");
                        // Vacation is over, return employee to work
                        await _employeeService.EndEmployeeVacationAsync(employee.Id);
                    }
                    else
                    {
                        Console.WriteLine($"Employee {employee.Name} vacation updated: {newDaysRemaining} days remaining");
                        // Update vacation end date to reflect one less day
                        var newEndDate = DateTime.Today.AddDays(newDaysRemaining);
                        Console.WriteLine($"Employee {employee.Name}: New vacation end date: {newEndDate:yyyy-MM-dd}");
                        employee.VacationEndDate = newEndDate;
                        await _employeeService.UpdateEmployee(employee);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the day advancement
            Console.WriteLine($"Error processing vacation days: {ex.Message}");
        }
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

    public async Task RestartGame()
    {
        // Reset to initial state
        _currentDay = 1;
        _companyMoney = 10000; // Starting money
        _moneyTransactions.Clear();
        _unlockedAchievements.Clear();
        _isSummaryBoardVisible = true; // Reset to default visible
        _isReadyForDevelopmentColumnVisible = true; // Reset to default visible

        // Trigger events to notify clients
        DayChanged?.Invoke(_currentDay);
        MoneyChanged?.Invoke(_companyMoney);
        SummaryBoardVisibilityChanged?.Invoke(_isSummaryBoardVisible);
        ReadyForDevelopmentColumnVisibilityChanged?.Invoke(_isReadyForDevelopmentColumnVisible);

        await Task.CompletedTask;
    }

    public async Task SetSummaryBoardVisibility(bool isVisible)
    {
        if (_isSummaryBoardVisible != isVisible)
        {
            _isSummaryBoardVisible = isVisible;

            // Notify all clients about the change via SignalR
            await _gameHub.Clients.All.SendAsync("SummaryBoardVisibilityChanged", _isSummaryBoardVisible);

            // Also trigger the local event
            SummaryBoardVisibilityChanged?.Invoke(_isSummaryBoardVisible);
        }
        await Task.CompletedTask;
    }

    public async Task SetReadyForDevelopmentColumnVisibility(bool isVisible)
    {
        if (_isReadyForDevelopmentColumnVisible != isVisible)
        {
            _isReadyForDevelopmentColumnVisible = isVisible;

            // Notify all clients about the change via SignalR
            await _gameHub.Clients.All.SendAsync("ReadyForDevelopmentColumnVisibilityChanged", _isReadyForDevelopmentColumnVisible);

            // Also trigger the local event
            ReadyForDevelopmentColumnVisibilityChanged?.Invoke(_isReadyForDevelopmentColumnVisible);
        }
        await Task.CompletedTask;
    }
}
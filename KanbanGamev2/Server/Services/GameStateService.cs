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
    private readonly IFeatureService _featureService;
    private readonly ITaskService _taskService;
    private readonly IHubContext<GameHub> _gameHub;
    private bool _isSummaryBoardVisible = true; // Default to visible
    private bool _isReadyForDevelopmentColumnVisible = true; // Default to visible

    public GameStateService(IEmployeeService employeeService, IFeatureService featureService, ITaskService taskService, IHubContext<GameHub> gameHub)
    {
        _employeeService = employeeService;
        _featureService = featureService;
        _taskService = taskService;
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

        // Process learning days for all employees
        await ProcessLearningDays();

        // Process team changing days for all employees
        await ProcessTeamChangingDays();

        // Process feature deadlines
        await ProcessFeatureDeadlines();

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

    private async Task ProcessLearningDays()
    {
        try
        {
            Console.WriteLine($"Processing learning days for day {_currentDay}...");

            // Get all employees currently learning
            var employees = _employeeService.GetEmployees();
            var employeesLearning = employees.Where(e => e.Status == EmployeeStatus.IsLearning || e.Status == EmployeeStatus.IsLearningInOtherTeam).ToList();

            Console.WriteLine($"Found {employeesLearning.Count} employees with learning status");

            foreach (var employee in employeesLearning)
            {
                // For IsLearningInOtherTeam, we don't check if they should be learning in the column
                // because they're learning in a different team - just process the learning days
                if (employee.Status == EmployeeStatus.IsLearningInOtherTeam)
                {
                    if (employee.LearningRole.HasValue)
                    {
                        // Increment learning days
                        employee.LearningDays++;
                        Console.WriteLine($"Employee {employee.Name} has been learning {employee.LearningRole.Value} in other team for {employee.LearningDays} days");

                        // Check if employee has learned the role (8 days)
                        if (employee.LearningDays >= Employee.DaysRequiredToLearnRole)
                        {
                            Console.WriteLine($"Employee {employee.Name} has learned the role {employee.LearningRole.Value}!");

                            // Add the role to learned roles
                            if (!employee.LearnedRoles.Contains(employee.LearningRole.Value))
                            {
                                employee.LearnedRoles.Add(employee.LearningRole.Value);
                            }

                            // Remove from learnable roles
                            employee.LearnableRoles.Remove(employee.LearningRole.Value);

                            // Reset learning state
                            employee.Status = EmployeeStatus.Active;
                            employee.LearningDays = 0;
                            employee.LearningRole = null;
                            employee.PreviousBoardType = null;

                            // Update employee
                            await _employeeService.UpdateEmployee(employee);
                        }
                        else
                        {
                            // Just update the learning days counter
                            await _employeeService.UpdateEmployee(employee);
                        }
                    }
                    continue;
                }

                // For regular IsLearning status, verify employee is still in a column where they should be learning
                if (!employee.ShouldBeLearningInColumn(employee.ColumnId))
                {
                    Console.WriteLine($"Employee {employee.Name} has IsLearning status but is in column {employee.ColumnId} where they shouldn't learn. Resetting status.");
                    employee.Status = EmployeeStatus.Active;
                    employee.LearningDays = 0;
                    employee.LearningRole = null;
                    await _employeeService.UpdateEmployee(employee);
                    continue;
                }

                // Ensure LearningRole is set based on current column
                var requiredRole = employee.GetRoleRequiredForColumn(employee.ColumnId);
                if (requiredRole == null)
                {
                    Console.WriteLine($"Employee {employee.Name} is learning but column {employee.ColumnId} doesn't require a role. Resetting status.");
                    employee.Status = EmployeeStatus.Active;
                    employee.LearningDays = 0;
                    employee.LearningRole = null;
                    await _employeeService.UpdateEmployee(employee);
                    continue;
                }

                // Set LearningRole if not set or if it changed
                if (!employee.LearningRole.HasValue || employee.LearningRole.Value != requiredRole.Value)
                {
                    Console.WriteLine($"Setting LearningRole for {employee.Name} to {requiredRole.Value}");
                    // Reset counter if role changed
                    if (employee.LearningRole.HasValue && employee.LearningRole.Value != requiredRole.Value)
                    {
                        employee.LearningDays = 0;
                    }
                    employee.LearningRole = requiredRole.Value;
                }

                // Increment learning days
                employee.LearningDays++;
                Console.WriteLine($"Employee {employee.Name} has been learning {employee.LearningRole.Value} for {employee.LearningDays} days in column {employee.ColumnId}");

                // Check if employee has learned the role (8 days)
                if (employee.LearningDays >= Employee.DaysRequiredToLearnRole)
                {
                    Console.WriteLine($"Employee {employee.Name} has learned the role {employee.LearningRole.Value}!");

                    // Add the role to learned roles
                    if (!employee.LearnedRoles.Contains(employee.LearningRole.Value))
                    {
                        employee.LearnedRoles.Add(employee.LearningRole.Value);
                    }

                    // Remove from learnable roles
                    employee.LearnableRoles.Remove(employee.LearningRole.Value);

                    // Reset learning state
                    employee.Status = EmployeeStatus.Active;
                    employee.LearningDays = 0;
                    employee.LearningRole = null;

                    // Update employee
                    await _employeeService.UpdateEmployee(employee);
                }
                else
                {
                    // Just update the learning days counter
                    await _employeeService.UpdateEmployee(employee);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the day advancement
            Console.WriteLine($"Error processing learning days: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private async Task ProcessTeamChangingDays()
    {
        try
        {
            Console.WriteLine($"Processing team changing days for day {_currentDay}...");

            // Get all employees currently changing teams
            var employees = _employeeService.GetEmployees();
            var employeesChangingTeams = employees.Where(e => e.Status == EmployeeStatus.ChangingTeams).ToList();

            Console.WriteLine($"Found {employeesChangingTeams.Count} employees changing teams");

            foreach (var employee in employeesChangingTeams)
            {
                // Increment team changing days
                employee.ChangingTeamsDays++;
                Console.WriteLine($"Employee {employee.Name} has been changing teams for {employee.ChangingTeamsDays} days (from {employee.PreviousBoardType} to {employee.BoardType})");

                // Check if employee has completed team change (3 days)
                if (employee.ChangingTeamsDays >= Employee.DaysRequiredToChangeTeams)
                {
                    Console.WriteLine($"Employee {employee.Name} has completed team change!");

                    // Reset team changing state
                    employee.Status = EmployeeStatus.Active;
                    employee.ChangingTeamsDays = 0;
                    employee.PreviousBoardType = null;

                    // Update employee
                    await _employeeService.UpdateEmployee(employee);
                }
                else
                {
                    // Just update the team changing days counter
                    await _employeeService.UpdateEmployee(employee);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the day advancement
            Console.WriteLine($"Error processing team changing days: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private DateTime GetCurrentGameDate()
    {
        return _gameStartDate.AddDays(_currentDay - 1);
    }

    private async Task ProcessFeatureDeadlines()
    {
        try
        {
            var currentGameDate = GetCurrentGameDate();
            Console.WriteLine($"Processing feature deadlines for day {_currentDay}...");
            Console.WriteLine($"Current game date: {currentGameDate:yyyy-MM-dd}");

            // Get all features with deadlines that are not yet delivered
            var features = _featureService.GetFeatures();
            var featuresWithDeadlines = features
                .Where(f => f.DueDate.HasValue && !f.IsDelivered)
                .ToList();

            Console.WriteLine($"Found {featuresWithDeadlines.Count} features with deadlines");

            foreach (var feature in featuresWithDeadlines)
            {
                if (feature.DueDate.HasValue)
                {
                    var deadlineDate = feature.DueDate.Value.Date;
                    var currentDate = currentGameDate.Date;

                    Console.WriteLine($"Feature '{feature.Title}': Deadline = {deadlineDate:yyyy-MM-dd}, Current Game Date = {currentDate:yyyy-MM-dd}");

                    // Check if deadline has passed (deadline is before or equal to current game date)
                    if (deadlineDate <= currentDate)
                    {
                        Console.WriteLine($"Feature '{feature.Title}' deadline has passed! Removing feature and all its tasks.");

                        // Unassign any employees working on this feature
                        var employees = _employeeService.GetEmployees();
                        var employeesAssignedToFeature = employees.Where(e => e.AssignedFeatureId == feature.Id).ToList();
                        foreach (var employee in employeesAssignedToFeature)
                        {
                            employee.AssignedFeatureId = null;
                            await _employeeService.UpdateEmployee(employee);
                            Console.WriteLine($"Unassigned employee '{employee.Name}' from expired feature '{feature.Title}'");
                        }

                        // Delete all tasks associated with this feature
                        if (feature.GeneratedTaskIds != null && feature.GeneratedTaskIds.Any())
                        {
                            Console.WriteLine($"Deleting {feature.GeneratedTaskIds.Count} tasks for feature '{feature.Title}'");

                            // Also unassign employees from tasks before deleting
                            var allTasks = _taskService.GetTasks();
                            var featureTasks = allTasks.Where(t => feature.GeneratedTaskIds.Contains(t.Id)).ToList();
                            foreach (var task in featureTasks)
                            {
                                if (task.AssignedToEmployeeId.HasValue)
                                {
                                    var taskEmployee = employees.FirstOrDefault(e => e.Id == task.AssignedToEmployeeId.Value);
                                    if (taskEmployee != null)
                                    {
                                        taskEmployee.AssignedTaskId = null;
                                        await _employeeService.UpdateEmployee(taskEmployee);
                                        Console.WriteLine($"Unassigned employee '{taskEmployee.Name}' from task '{task.Title}'");
                                    }
                                }
                                _taskService.DeleteTask(task.Id);
                            }
                        }

                        // Delete the feature itself
                        await _featureService.DeleteFeature(feature.Id);

                        Console.WriteLine($"Feature '{feature.Title}' and all its tasks have been removed due to missed deadline. No profit awarded.");

                        // Notify all connected users to refresh their boards
                        await _gameHub.Clients.All.SendAsync("RefreshAllBoards");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the day advancement
            Console.WriteLine($"Error processing feature deadlines: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
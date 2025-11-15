using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class EmployeeService : IEmployeeService
{
    private List<Employee> _employees;
    private readonly INotificationService _notificationService;

    public EmployeeService(INotificationService notificationService)
    {
        _notificationService = notificationService;
        _employees = new List<Employee>();
        SeedData();
    }

    public List<Employee> GetEmployees()
    {
        return _employees;
    }

    public Employee? GetEmployee(Guid id)
    {
        return _employees.FirstOrDefault(e => e.Id == id);
    }

    public List<Employee> GetAvailableEmployees()
    {
        return _employees.Where(e => e.IsAvailable).ToList();
    }

    public async Task<Employee> CreateEmployee(Employee employee)
    {
        employee.Id = Guid.NewGuid();
        employee.CreatedAt = DateTime.Now;
        
        // Set onboarding status for 5 days
        employee.Status = EmployeeStatus.Onboarding;
        employee.OnboardingEndDate = DateTime.Now.AddDays(5);
        employee.IsAvailable = false; // Onboarding employees cannot work
        
        // Set default wage if not provided
        if (employee.MonthlyWage == 0)
        {
            employee.MonthlyWage = GetDefaultWageForSeniority(employee.Seniority);
        }
        
        _employees.Add(employee);
        
        // Notify all clients about the new employee
        await _notificationService.NotifyEmployeeCreatedAsync(employee);
        await _notificationService.NotifyRefreshAllBoardsAsync();
        
        return employee;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        var existingEmployee = _employees.FirstOrDefault(e => e.Id == employee.Id);
        if (existingEmployee == null)
            throw new ArgumentException("Employee not found");

        // Update properties
        existingEmployee.Name = employee.Name;
        existingEmployee.Email = employee.Email;
        existingEmployee.LearnedRoles = employee.LearnedRoles;
        existingEmployee.LearnableRoles = employee.LearnableRoles;
        existingEmployee.Department = employee.Department;
        existingEmployee.Seniority = employee.Seniority;
        existingEmployee.ColumnId = employee.ColumnId;
        existingEmployee.BoardType = employee.BoardType;
        existingEmployee.Status = employee.Status;
        existingEmployee.VacationStartDate = employee.VacationStartDate;
        existingEmployee.VacationEndDate = employee.VacationEndDate;
        existingEmployee.OnboardingEndDate = employee.OnboardingEndDate;
        existingEmployee.MonthlyWage = employee.MonthlyWage;
        
        // Update learning properties
        existingEmployee.LearningDays = employee.LearningDays;
        existingEmployee.LearningRole = employee.LearningRole;
        
        // Update team changing properties
        existingEmployee.ChangingTeamsDays = employee.ChangingTeamsDays;
        existingEmployee.PreviousBoardType = employee.PreviousBoardType;
        
        // Update work assignment properties
        existingEmployee.AssignedTaskId = employee.AssignedTaskId;
        existingEmployee.AssignedFeatureId = employee.AssignedFeatureId;

        return existingEmployee;
    }

    public async Task<bool> DeleteEmployee(Guid id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return false;

        return _employees.Remove(employee);
    }

    private void SeedData()
    {
        _employees = new List<Employee>
        {
            // Analysis Board Employees
            // analysis1 - HighLevelAnalyst
            CreateEmployeeWithRoles("Alex", "alex@company.com", "analysis1", BoardType.Analysis, 
                Role.HighLevelAnalyst, 
                new List<Role> { Role.Analyst, Role.Developer }, 
                Role.Tester),
            
            // analysis2 - HighLevelAnalyst (same as analysis1)
            CreateEmployeeWithRoles("Alice", "alice@company.com", "analysis2", BoardType.Analysis, 
                Role.HighLevelAnalyst, 
                new List<Role> { Role.Developer, Role.Tester }, 
                Role.Analyst),
            
            // Backend Board Employees
            // backend-analysis - Analyst
            CreateEmployeeWithRoles("Brian", "brian@company.com", "backend-analysis", BoardType.Backend, 
                Role.Analyst, 
                new List<Role> { Role.Developer, Role.Tester }, 
                Role.HighLevelAnalyst),
            
            // backend-dev-doing - Developer
            CreateEmployeeWithRoles("Charles", "charles@company.com", "backend-dev-doing", BoardType.Backend, 
                Role.Developer, 
                new List<Role> { Role.Analyst, Role.Tester }, 
                Role.HighLevelAnalyst),
            
            // backend-test-doing - Tester
            CreateEmployeeWithRoles("Claire", "claire@company.com", "backend-test-doing", BoardType.Backend, 
                Role.Tester, 
                new List<Role> { Role.Analyst, Role.Developer }, 
                Role.HighLevelAnalyst),
            
            // Frontend Board Employees
            // frontend-analysis - Analyst
            CreateEmployeeWithRoles("Diana", "diana@company.com", "frontend-analysis", BoardType.Frontend, 
                Role.Analyst, 
                new List<Role> { Role.Developer, Role.Tester }, 
                Role.HighLevelAnalyst),
            
            // frontend-dev-doing - Developer
            CreateEmployeeWithRoles("Emma", "emma@company.com", "frontend-dev-doing", BoardType.Frontend, 
                Role.Developer, 
                new List<Role> { Role.Analyst, Role.Tester }, 
                Role.HighLevelAnalyst),
            
            // frontend-test-doing - Tester
            CreateEmployeeWithRoles("Ethan", "ethan@company.com", "frontend-test-doing", BoardType.Frontend, 
                Role.Tester, 
                new List<Role> { Role.Analyst, Role.Developer }, 
                Role.HighLevelAnalyst)
        };
    }

    private Employee CreateEmployeeWithRoles(string name, string email, string columnId, BoardType boardType, 
        Role learnedRole, List<Role> learnableRoles, Role blockedRole)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            LearnedRoles = new List<Role> { learnedRole },
            LearnableRoles = learnableRoles,
            Department = Department.Engineering,
            Seniority = Seniority.Mid,
            BoardType = boardType,
            ColumnId = columnId,
            Status = EmployeeStatus.Active
        };
        
        // Set default wage based on seniority
        employee.MonthlyWage = GetDefaultWageForSeniority(employee.Seniority);
        
        return employee;
    }

    private decimal GetDefaultWageForSeniority(Seniority seniority)
    {
        return seniority switch
        {
            Seniority.Junior => 2000,
            Seniority.Mid => 3500,
            Seniority.Senior => 5000,
            _ => 2000
        };
    }

    public void ResetData()
    {
        _employees.Clear();
        SeedData();
    }

    public bool UnassignWorkFromEmployee(Guid id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee != null)
        {
            employee.AssignedTaskId = null;
            employee.AssignedFeatureId = null;
            employee.UpdatedAt = DateTime.Now;
            return true;
        }
        return false;
    }

    public bool MoveEmployee(Guid id, BoardType boardType, string columnId)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return false;

        var oldColumnId = employee.ColumnId;
        var oldBoardType = employee.BoardType;
        employee.ColumnId = columnId;
        employee.BoardType = boardType;
        
        // Check if employee should be learning in the new column
        bool shouldBeLearning = employee.ShouldBeLearningInColumn(columnId);
        
        // Check if employee is changing teams (moving to a different board)
        if (oldBoardType != boardType)
        {
            // Reset learning state if they were learning
            if (employee.Status == EmployeeStatus.IsLearning)
            {
                employee.LearningDays = 0;
                employee.LearningRole = null;
            }
            
            // If moving to different team to learn a role, skip ChangingTeams and start learning
            if (shouldBeLearning)
            {
                var requiredRole = employee.GetRoleRequiredForColumn(columnId);
                employee.Status = EmployeeStatus.IsLearningInOtherTeam; // Set to learning in other team
                employee.LearningDays = 0;
                employee.LearningRole = requiredRole;
                employee.PreviousBoardType = oldBoardType; // Store previous board type
            }
            else
            {
                // Employee is changing teams but not learning - set status to ChangingTeams
                employee.Status = EmployeeStatus.ChangingTeams;
                employee.ChangingTeamsDays = 0;
                employee.PreviousBoardType = oldBoardType;
            }
        }
        // Same board - check if employee should be learning
        else if (shouldBeLearning)
        {
            var requiredRole = employee.GetRoleRequiredForColumn(columnId);
            
            // If moving to a different column or different role, reset learning counter
            if (oldColumnId != columnId || employee.LearningRole != requiredRole)
            {
                employee.LearningDays = 0;
            }
            
            employee.Status = EmployeeStatus.IsLearning;
            employee.LearningRole = requiredRole;
        }
        else if (employee.Status == EmployeeStatus.IsLearning)
        {
            // If employee was learning but no longer should be, reset learning state
            employee.Status = EmployeeStatus.Active;
            employee.LearningDays = 0;
            employee.LearningRole = null;
        }
        else if (employee.Status == EmployeeStatus.ChangingTeams && oldBoardType == boardType)
        {
            // If employee was changing teams but is now in the same board, keep the status
            // (they need to complete the team change period)
        }
        else if (employee.Status != EmployeeStatus.ChangingTeams)
        {
            // Otherwise set to Active if not already changing teams
            employee.Status = EmployeeStatus.Active;
        }
        
        employee.UpdatedAt = DateTime.Now;
        return true;
    }

    public async Task<bool> SendEmployeeOnVacationAsync(Guid employeeId, int days)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;

        // Unassign any current work
        UnassignWorkFromEmployee(employeeId);

        // Set vacation status
        employee.Status = EmployeeStatus.OnVacation;
        employee.VacationStartDate = DateTime.Now;
        employee.VacationEndDate = DateTime.Now.AddDays(days);

        // Send notification and status change signal
        await _notificationService.SendGlobalNotificationAsync("Employee Vacation",
            $"{employee.Name} has been sent on vacation for {days} days.",
            "Warning");

        await _notificationService.NotifyEmployeeStatusChangedAsync(employee, oldStatus, employee.Status);

        return true;
    }

    public async Task<bool> EndEmployeeVacationAsync(Guid employeeId)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;

        // Clear vacation status
        employee.Status = EmployeeStatus.Active;
        employee.VacationStartDate = null;
        employee.VacationEndDate = null;

        // Send notification and status change signal
        await _notificationService.SendGlobalNotificationAsync("Vacation Ended",
            $"{employee.Name} has returned from vacation and is now active.",
            "Success");

        await _notificationService.NotifyEmployeeStatusChangedAsync(employee, oldStatus, employee.Status);

        return true;
    }

    public async Task<bool> EndEmployeeOnboardingAsync(Guid employeeId)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;

        // Clear onboarding status
        employee.Status = EmployeeStatus.Active;
        employee.OnboardingEndDate = null;
        employee.IsAvailable = true; // Now available to work

        // Send notification and status change signal
        await _notificationService.SendGlobalNotificationAsync("Onboarding Completed",
            $"{employee.Name} has completed onboarding and is now active.",
            "Success");

        await _notificationService.NotifyEmployeeStatusChangedAsync(employee, oldStatus, employee.Status);
        await _notificationService.NotifyRefreshAllBoardsAsync();

        return true;
    }

    public async Task<bool> FireEmployeeAsync(Guid employeeId)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;

        // Unassign any current work
        UnassignWorkFromEmployee(employeeId);

        // Set fired status
        employee.Status = EmployeeStatus.Fired;
        employee.VacationStartDate = null;
        employee.VacationEndDate = null;

        // Send notification and status change signal
        await _notificationService.SendGlobalNotificationAsync("Employee Fired",
            $"{employee.Name} has been fired from the company.",
            "Error");

        await _notificationService.NotifyEmployeeStatusChangedAsync(employee, oldStatus, employee.Status);

        return true;
    }

    public async Task<bool> RehireEmployeeAsync(Guid employeeId)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status; // Capture old status

        // Reset employee status
        employee.Status = EmployeeStatus.Active;
        employee.VacationStartDate = null;
        employee.VacationEndDate = null;

        // Send notification and status change signal
        await _notificationService.SendGlobalNotificationAsync("Employee Rehired",
            $"{employee.Name} has been rehired and is now active.",
            "Success");
        await _notificationService.NotifyEmployeeStatusChangedAsync(employee, oldStatus, employee.Status);

        return true;
    }

    public async Task<List<Employee>> GetEmployeesByColumnAsync(string columnId)
    {
        return _employees.Where(e => e.ColumnId == columnId && (e.Status == EmployeeStatus.Active || e.Status == EmployeeStatus.Onboarding || e.Status == EmployeeStatus.IsLearning || e.Status == EmployeeStatus.ChangingTeams || e.Status == EmployeeStatus.IsLearningInOtherTeam)).ToList();
    }

    public async Task<List<Employee>> GetAvailableEmployeesAsync()
    {
        return _employees.Where(e => e.Status == EmployeeStatus.Active && !e.IsWorking && !e.IsOnboarding).ToList();
    }
}
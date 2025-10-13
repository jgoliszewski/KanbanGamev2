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

    public Employee CreateEmployee(Employee employee)
    {
        employee.Id = Guid.NewGuid();
        employee.CreatedAt = DateTime.Now;
        _employees.Add(employee);
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
        existingEmployee.Status = employee.Status;
        existingEmployee.VacationStartDate = employee.VacationStartDate;
        existingEmployee.VacationEndDate = employee.VacationEndDate;
        
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
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex",
                Email = "alex@company.com",
                LearnedRoles = new List<Role> { Role.Analyst },
                LearnableRoles = new List<Role> { Role.HighLevelAnalyst, Role.Tester },
                Department = Department.Engineering,
                Seniority = Seniority.Mid,
                BoardType = BoardType.Analysis,
                ColumnId = "analysis1",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Email = "alice@company.com",
                LearnedRoles = new List<Role> { Role.HighLevelAnalyst },
                LearnableRoles = new List<Role> { Role.Analyst, Role.Tester },
                Department = Department.Engineering,
                Seniority = Seniority.Senior,
                BoardType = BoardType.Analysis,
                ColumnId = "analysis2",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Beth",
                Email = "beth@company.com",
                LearnedRoles = new List<Role> { Role.Analyst, Role.Tester },
                LearnableRoles = new List<Role> { Role.Developer, Role.HighLevelAnalyst },
                Department = Department.Engineering,
                Seniority = Seniority.Mid,
                BoardType = BoardType.Analysis,
                ColumnId = "analysis1",
                Status = EmployeeStatus.Active
            },
            
            // Backend Board Employees
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Brian",
                Email = "brian@company.com",
                LearnedRoles = new List<Role> { Role.Developer },
                LearnableRoles = new List<Role> { Role.Analyst, Role.Tester },
                Department = Department.Engineering,
                Seniority = Seniority.Senior,
                BoardType = BoardType.Backend,
                ColumnId = "backend-dev-doing",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Charles",
                Email = "charles@company.com",
                LearnedRoles = new List<Role> { Role.Tester },
                LearnableRoles = new List<Role> { Role.Analyst, Role.Developer },
                Department = Department.Engineering,
                Seniority = Seniority.Junior,
                BoardType = BoardType.Backend,
                ColumnId = "backend-test-doing",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Claire",
                Email = "claire@company.com",
                LearnedRoles = new List<Role> { Role.Analyst, Role.Developer },
                LearnableRoles = new List<Role> { Role.HighLevelAnalyst, Role.Tester },
                Department = Department.Engineering,
                Seniority = Seniority.Mid,
                BoardType = BoardType.Backend,
                ColumnId = "backend-analysis",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "David",
                Email = "david@company.com",
                LearnedRoles = new List<Role> { Role.Developer, Role.HighLevelAnalyst },
                LearnableRoles = new List<Role> { Role.Analyst, Role.Tester },
                Department = Department.Engineering,
                Seniority = Seniority.Senior,
                BoardType = BoardType.Backend,
                ColumnId = "backend-dev-doing",
                Status = EmployeeStatus.Active
            },
            
            // Frontend Board Employees
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Diana",
                Email = "diana@company.com",
                LearnedRoles = new List<Role> { Role.Developer },
                LearnableRoles = new List<Role> { Role.Analyst, Role.Tester },
                Department = Department.Engineering,
                Seniority = Seniority.Mid,
                BoardType = BoardType.Frontend,
                ColumnId = "frontend-dev-doing",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Emma",
                Email = "emma@company.com",
                LearnedRoles = new List<Role> { Role.Developer, Role.Tester },
                LearnableRoles = new List<Role> { Role.Analyst, Role.HighLevelAnalyst },
                Department = Department.Engineering,
                Seniority = Seniority.Senior,
                BoardType = BoardType.Frontend,
                ColumnId = "frontend-test-doing",
                Status = EmployeeStatus.Active
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Ethan",
                Email = "ethan@company.com",
                LearnedRoles = new List<Role> { Role.Tester, Role.Analyst },
                LearnableRoles = new List<Role> { Role.Developer, Role.HighLevelAnalyst },
                Department = Department.Engineering,
                Seniority = Seniority.Junior,
                BoardType = BoardType.Frontend,
                ColumnId = "frontend-analysis",
                Status = EmployeeStatus.Active
            }
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

        employee.ColumnId = columnId;
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
        return _employees.Where(e => e.ColumnId == columnId && e.Status == EmployeeStatus.Active).ToList();
    }

    public async Task<List<Employee>> GetAvailableEmployeesAsync()
    {
        return _employees.Where(e => e.Status == EmployeeStatus.Active && !e.IsWorking).ToList();
    }
}
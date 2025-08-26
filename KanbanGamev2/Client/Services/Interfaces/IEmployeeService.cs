using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface IEmployeeService
{
    List<Employee> Employees { get; set; }
    Task GetEmployees();
    Task<Employee?> GetEmployee(Guid id);
    Task<Employee> CreateEmployee(Employee employee);
    Task<Employee> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(Guid id);
    Task<List<Employee>> GetEmployeesByColumnAsync(string columnId);
    Task<List<Employee>> GetAvailableEmployeesAsync();
    
    // New Game Master methods
    Task<bool> SendEmployeeOnVacationAsync(Guid employeeId, int days);
    Task<bool> EndEmployeeVacationAsync(Guid employeeId);
    Task<bool> FireEmployeeAsync(Guid employeeId);
    Task<bool> RehireEmployeeAsync(Guid employeeId);
    Task<bool> UnassignWorkFromEmployee(Guid employeeId);
    Task UpdateEmployees();
    Task<bool> MoveEmployee(Guid employeeId, BoardType boardType, string columnId);
} 
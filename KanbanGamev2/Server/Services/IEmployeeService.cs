using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface IEmployeeService
{
    Task<List<Employee>> GetEmployeesAsync();
    Task<Employee?> GetEmployeeAsync(Guid id);
    Task<Employee> CreateEmployeeAsync(Employee employee);
    Task<Employee> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(Guid id);
    Task<List<Employee>> GetAvailableEmployeesAsync();
} 
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
    Task<List<Employee>> GetAvailableEmployees();
    Task<bool> UnassignWorkFromEmployee(Guid employeeId);
    Task UpdateEmployees();
} 
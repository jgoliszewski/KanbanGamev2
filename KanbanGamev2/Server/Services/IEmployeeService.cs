using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface IEmployeeService
{
    List<Employee> GetEmployees();
    Employee? GetEmployee(Guid id);
    List<Employee> GetAvailableEmployees();
    Employee CreateEmployee(Employee employee);
    Task<Employee> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(Guid id);
    List<Employee> GetEmployeesByColumn(string columnId);
    void ResetData();
    bool UnassignWorkFromEmployee(Guid id);
    bool MoveEmployee(Guid id, BoardType boardType, string columnId);
    Task<List<Employee>> GetEmployeesByColumnAsync(string columnId);
    Task<List<Employee>> GetAvailableEmployeesAsync();
    
    // New Game Master methods
    Task<bool> SendEmployeeOnVacationAsync(Guid employeeId, int days);
    Task<bool> EndEmployeeVacationAsync(Guid employeeId);
    Task<bool> FireEmployeeAsync(Guid employeeId);
    Task<bool> RehireEmployeeAsync(Guid employeeId);
    Task CheckAndProcessVacationEndsAsync();
} 
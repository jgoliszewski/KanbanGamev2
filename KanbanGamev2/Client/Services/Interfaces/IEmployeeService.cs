using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface IEmployeeService
{
    List<Employee> Employees { get; set; }
    Task GetEmployees();
    Task<Employee> UpdateEmployee(Employee employee);

    // New Game Master methods
    Task<bool> SendEmployeeOnVacationAsync(Guid employeeId, int days);
    Task<bool> EndEmployeeVacationAsync(Guid employeeId);
    Task<bool> FireEmployeeAsync(Guid employeeId);
    Task<bool> RehireEmployeeAsync(Guid employeeId);
    Task<bool> MoveEmployee(Guid employeeId, BoardType boardType, string columnId);
}
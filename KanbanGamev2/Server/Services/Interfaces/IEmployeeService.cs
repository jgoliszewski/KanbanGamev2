using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface IEmployeeService
{
    List<Employee> GetEmployees();
    Employee? GetEmployee(Guid id);
    List<Employee> GetAvailableEmployees();
    Task<Employee> CreateEmployee(Employee employee);
    Task<Employee> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(Guid id);
    void ResetData();
    bool UnassignWorkFromEmployee(Guid id);
    bool MoveEmployee(Guid id, BoardType boardType, string columnId);

    Task<bool> SendEmployeeOnVacationAsync(Guid employeeId, int days);
    Task<bool> EndEmployeeVacationAsync(Guid employeeId);
    Task<bool> EndEmployeeOnboardingAsync(Guid employeeId);
    Task<bool> FireEmployeeAsync(Guid employeeId);
    Task<bool> RehireEmployeeAsync(Guid employeeId);
}
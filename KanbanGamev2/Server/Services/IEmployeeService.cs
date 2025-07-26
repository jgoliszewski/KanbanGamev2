using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface IEmployeeService
{
    List<Employee> GetEmployees();
    Employee? GetEmployee(Guid id);
    List<Employee> GetAvailableEmployees();
    Employee CreateEmployee(Employee employee);
    Employee UpdateEmployee(Employee employee);
    bool DeleteEmployee(Guid id);
    List<Employee> GetEmployeesByColumn(string columnId);
    void ResetData();
    bool UnassignWorkFromEmployee(Guid id);
} 
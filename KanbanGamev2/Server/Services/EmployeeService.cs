using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class EmployeeService : IEmployeeService
{
    private List<Employee> _employees = new();

    public EmployeeService()
    {
        SeedData();
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await Task.FromResult(_employees);
    }

    public async Task<Employee?> GetEmployeeAsync(Guid id)
    {
        return await Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        employee.Id = Guid.NewGuid();
        employee.CreatedAt = DateTime.Now;
        _employees.Add(employee);
        return await Task.FromResult(employee);
    }

    public async Task<Employee> UpdateEmployeeAsync(Employee employee)
    {
        var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
        if (existing != null)
        {
            existing.Title = employee.Title;
            existing.Description = employee.Description;
            existing.Name = employee.Name;
            existing.Role = employee.Role;
            existing.Department = employee.Department;
            existing.Email = employee.Email;
            existing.IsAvailable = employee.IsAvailable;
            existing.ColumnId = employee.ColumnId;
            existing.Order = employee.Order;
            existing.UpdatedAt = DateTime.Now;
            return await Task.FromResult(existing);
        }
        throw new ArgumentException("Employee not found");
    }

    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee != null)
        {
            _employees.Remove(employee);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<List<Employee>> GetAvailableEmployeesAsync()
    {
        return await Task.FromResult(_employees.Where(e => e.IsAvailable).ToList());
    }

    private void SeedData()
    {
        _employees = new List<Employee>
        {
            new Employee
            {
                Id = Guid.NewGuid(),
                Title = "John Doe",
                Name = "John Doe",
                Role = "Senior Developer",
                Department = "Engineering",
                Email = "john.doe@company.com",
                IsAvailable = true,
                ColumnId = "analysis1",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Title = "Jane Smith",
                Name = "Jane Smith",
                Role = "UI/UX Designer",
                Department = "Design",
                Email = "jane.smith@company.com",
                IsAvailable = true,
                ColumnId = "backend-dev-doing",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Title = "Mike Johnson",
                Name = "Mike Johnson",
                Role = "Product Manager",
                Department = "Product",
                Email = "mike.johnson@company.com",
                IsAvailable = false,
                ColumnId = "frontend-dev-waiting",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Title = "Sarah Wilson",
                Name = "Sarah Wilson",
                Role = "QA Engineer",
                Department = "Quality Assurance",
                Email = "sarah.wilson@company.com",
                IsAvailable = true,
                ColumnId = "backend-test-doing",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Title = "David Brown",
                Name = "David Brown",
                Role = "Frontend Developer",
                Department = "Engineering",
                Email = "david.brown@company.com",
                IsAvailable = true,
                ColumnId = "frontend-dev-doing",
                Order = 1
            }
        };
    }

    public void ResetData()
    {
        _employees.Clear();
        SeedData();
    }
} 
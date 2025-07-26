using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class EmployeeService : IEmployeeService
{
    private List<Employee> _employees = new();

    public EmployeeService()
    {
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

    public Employee UpdateEmployee(Employee employee)
    {
        var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
        if (existing != null)
        {
            existing.Name = employee.Name;
            existing.Role = employee.Role;
            existing.Department = employee.Department;
            existing.Email = employee.Email;
            existing.IsAvailable = employee.IsAvailable;
            existing.ColumnId = employee.ColumnId;
            existing.Order = employee.Order;
            existing.UpdatedAt = DateTime.Now;
            return existing;
        }
        throw new ArgumentException("Employee not found");
    }

    public bool DeleteEmployee(Guid id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee != null)
        {
            _employees.Remove(employee);
            return true;
        }
        return false;
    }

    public List<Employee> GetEmployeesByColumn(string columnId)
    {
        return _employees.Where(e => e.ColumnId == columnId).OrderBy(e => e.Order).ToList();
    }

    private void SeedData()
    {
        _employees = new List<Employee>
        {
            // Analysis Board - Employees can only be in analysis1 and analysis2
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex Turner",
                Role = Role.SeniorDeveloper,
                Department = Department.Engineering,
                Email = "alex.turner@company.com",
                IsAvailable = true,
                ColumnId = "analysis1",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Emma Davis",
                Role = Role.ProductManager,
                Department = Department.Product,
                Email = "emma.davis@company.com",
                IsAvailable = true,
                ColumnId = "analysis2",
                Order = 1
            },
            
            // Backend Board - Employees can only be in analysis, backend-dev-doing, backend-test-doing
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Beth Cooper",
                Role = Role.LeadDeveloper,
                Department = Department.Engineering,
                Email = "beth.cooper@company.com",
                IsAvailable = false,
                ColumnId = "backend-dev-doing",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "David Brown",
                Role = Role.QAEngineer,
                Department = Department.QualityAssurance,
                Email = "david.brown@company.com",
                IsAvailable = true,
                ColumnId = "backend-test-doing",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Henry Chen",
                Role = Role.DevOpsEngineer,
                Department = Department.Engineering,
                Email = "henry.chen@company.com",
                IsAvailable = true,
                ColumnId = "backend-analysis",
                Order = 1
            },
            
            // Frontend Board - Employees can only be in frontend-analysis, frontend-dev-doing, frontend-test-doing
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Claire Bennett",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "claire.bennett@company.com",
                IsAvailable = true,
                ColumnId = "frontend-dev-doing",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Frank Miller",
                Role = Role.UIDesigner,
                Department = Department.Design,
                Email = "frank.miller@company.com",
                IsAvailable = true,
                ColumnId = "frontend-analysis",
                Order = 1
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Grace Lee",
                Role = Role.UXDesigner,
                Department = Department.Design,
                Email = "grace.lee@company.com",
                IsAvailable = false,
                ColumnId = "frontend-test-doing",
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
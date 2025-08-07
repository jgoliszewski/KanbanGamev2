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
            existing.AssignedTaskId = employee.AssignedTaskId;
            existing.AssignedFeatureId = employee.AssignedFeatureId;
            existing.Seniority = employee.Seniority;
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
            // Analysis Board - 2 employees in each worker column
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex Turner",
                Role = Role.SeniorDeveloper,
                Department = Department.Engineering,
                Email = "alex.turner@company.com",
                IsAvailable = true,
                ColumnId = "analysis1",
                Order = 1,
                Seniority = Seniority.Senior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Emma Davis",
                Role = Role.ProductManager,
                Department = Department.Product,
                Email = "emma.davis@company.com",
                IsAvailable = true,
                ColumnId = "analysis1",
                Order = 2,
                Seniority = Seniority.Mid
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Ivy Rodriguez",
                Role = Role.BusinessAnalyst,
                Department = Department.Product,
                Email = "ivy.rodriguez@company.com",
                IsAvailable = true,
                ColumnId = "analysis2",
                Order = 1,
                Seniority = Seniority.Senior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Jack Thompson",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "jack.thompson@company.com",
                IsAvailable = true,
                ColumnId = "analysis2",
                Order = 2,
                Seniority = Seniority.Junior
            },
            
            // Backend Board - 2 employees in each worker column
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Beth Cooper",
                Role = Role.LeadDeveloper,
                Department = Department.Engineering,
                Email = "beth.cooper@company.com",
                IsAvailable = true,
                ColumnId = "backend-analysis",
                Order = 1,
                Seniority = Seniority.Senior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Kevin O'Brien",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "kevin.obrien@company.com",
                IsAvailable = true,
                ColumnId = "backend-analysis",
                Order = 2,
                Seniority = Seniority.Mid
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Liam Anderson",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "liam.anderson@company.com",
                IsAvailable = true,
                ColumnId = "backend-dev-doing",
                Order = 1,
                Seniority = Seniority.Senior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Mark Taylor",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "mark.taylor@company.com",
                IsAvailable = true,
                ColumnId = "backend-dev-doing",
                Order = 2,
                Seniority = Seniority.Junior
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
                Order = 1,
                Seniority = Seniority.Mid
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Nathan Garcia",
                Role = Role.QAEngineer,
                Department = Department.QualityAssurance,
                Email = "nathan.garcia@company.com",
                IsAvailable = true,
                ColumnId = "backend-test-doing",
                Order = 2,
                Seniority = Seniority.Junior
            },
            
            // Frontend Board - 2 employees in each worker column
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Claire Bennett",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "claire.bennett@company.com",
                IsAvailable = true,
                ColumnId = "frontend-analysis",
                Order = 1,
                Seniority = Seniority.Mid
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Oliver Martinez",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "oliver.martinez@company.com",
                IsAvailable = true,
                ColumnId = "frontend-analysis",
                Order = 2,
                Seniority = Seniority.Junior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Frank Miller",
                Role = Role.UIDesigner,
                Department = Department.Design,
                Email = "frank.miller@company.com",
                IsAvailable = true,
                ColumnId = "frontend-dev-doing",
                Order = 1,
                Seniority = Seniority.Senior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Paul Robinson",
                Role = Role.Developer,
                Department = Department.Engineering,
                Email = "paul.robinson@company.com",
                IsAvailable = true,
                ColumnId = "frontend-dev-doing",
                Order = 2,
                Seniority = Seniority.Mid
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Grace Lee",
                Role = Role.UXDesigner,
                Department = Department.Design,
                Email = "grace.lee@company.com",
                IsAvailable = true,
                ColumnId = "frontend-test-doing",
                Order = 1,
                Seniority = Seniority.Senior
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Quentin White",
                Role = Role.QAEngineer,
                Department = Department.QualityAssurance,
                Email = "quentin.white@company.com",
                IsAvailable = true,
                ColumnId = "frontend-test-doing",
                Order = 2,
                Seniority = Seniority.Junior
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

    public async Task UpdateEmployees()
    {
        // This method is called after work simulation to persist changes
        // In a real application, this would save to a database
        await Task.CompletedTask;
    }

    public bool MoveEmployee(Guid id, BoardType boardType, string columnId)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee != null)
        {
            employee.ColumnId = columnId;
            employee.UpdatedAt = DateTime.Now;
            return true;
        }
        return false;
    }
} 
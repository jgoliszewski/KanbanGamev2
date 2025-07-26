using Microsoft.AspNetCore.Mvc;
using KanbanGame.Shared;
using KanbanGamev2.Server.Services;

namespace KanbanGamev2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetEmployees()
    {
        var employees = await _employeeService.GetEmployeesAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(Guid id)
    {
        var employee = await _employeeService.GetEmployeeAsync(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<Employee>>> GetAvailableEmployees()
    {
        var employees = await _employeeService.GetAvailableEmployeesAsync();
        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
        var created = await _employeeService.CreateEmployeeAsync(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Employee>> UpdateEmployee(Guid id, Employee employee)
    {
        if (id != employee.Id)
            return BadRequest();

        try
        {
            var updated = await _employeeService.UpdateEmployeeAsync(employee);
            return Ok(updated);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
} 
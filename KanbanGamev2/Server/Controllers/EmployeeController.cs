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
    public ActionResult<List<Employee>> GetEmployees()
    {
        var employees = _employeeService.GetEmployees();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public ActionResult<Employee> GetEmployee(Guid id)
    {
        var employee = _employeeService.GetEmployee(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpGet("available")]
    public ActionResult<List<Employee>> GetAvailableEmployees()
    {
        var employees = _employeeService.GetAvailableEmployees();
        return Ok(employees);
    }

    [HttpPost]
    public ActionResult<Employee> CreateEmployee(Employee employee)
    {
        var created = _employeeService.CreateEmployee(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<Employee> UpdateEmployee(Guid id, Employee employee)
    {
        if (id != employee.Id)
            return BadRequest();

        try
        {
            var updated = _employeeService.UpdateEmployee(employee);
            return Ok(updated);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteEmployee(Guid id)
    {
        var deleted = _employeeService.DeleteEmployee(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/unassign")]
    public ActionResult UnassignWorkFromEmployee(Guid id)
    {
        var success = _employeeService.UnassignWorkFromEmployee(id);
        if (!success)
            return NotFound();
        return NoContent();
    }
} 
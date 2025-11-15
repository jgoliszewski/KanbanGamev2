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
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
        var created = await _employeeService.CreateEmployee(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Employee>> UpdateEmployee(Guid id, Employee employee)
    {
        if (id != employee.Id)
            return BadRequest();

        var updatedEmployee = await _employeeService.UpdateEmployee(employee);
        return Ok(updatedEmployee);
    }

    [HttpPost("{id}/vacation")]
    public async Task<ActionResult> SendEmployeeOnVacation(Guid id, [FromQuery] int days)
    {
        var result = await _employeeService.SendEmployeeOnVacationAsync(id, days);
        if (result)
            return Ok();
        return NotFound();
    }

    [HttpPost("{id}/end-vacation")]
    public async Task<ActionResult> EndEmployeeVacation(Guid id)
    {
        var result = await _employeeService.EndEmployeeVacationAsync(id);
        if (result)
            return Ok();
        return NotFound();
    }

    [HttpPost("{id}/fire")]
    public async Task<ActionResult> FireEmployee(Guid id)
    {
        var result = await _employeeService.FireEmployeeAsync(id);
        if (result)
            return Ok();
        return NotFound();
    }

    [HttpPost("{id}/rehire")]
    public async Task<ActionResult> RehireEmployee(Guid id)
    {
        var result = await _employeeService.RehireEmployeeAsync(id);
        if (result)
            return Ok();
        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        var deleted = await _employeeService.DeleteEmployee(id);
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

    [HttpPut("{id}/move")]
    public ActionResult MoveEmployee(Guid id, [FromQuery] BoardType boardType, [FromQuery] string columnId)
    {
        var success = _employeeService.MoveEmployee(id, boardType, columnId);
        if (!success)
            return NotFound();
        return NoContent();
    }
} 
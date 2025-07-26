using Microsoft.AspNetCore.Mvc;
using KanbanGame.Shared;
using KanbanGamev2.Server.Services;

namespace KanbanGamev2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<KanbanTask>>> GetTasks()
    {
        var tasks = await _taskService.GetTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KanbanTask>> GetTask(Guid id)
    {
        var task = await _taskService.GetTaskAsync(id);
        if (task == null)
            return NotFound();
        return Ok(task);
    }

    [HttpGet("column/{columnId}")]
    public async Task<ActionResult<List<KanbanTask>>> GetTasksByColumn(string columnId)
    {
        var tasks = await _taskService.GetTasksByColumnAsync(columnId);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<KanbanTask>> CreateTask(KanbanTask task)
    {
        var created = await _taskService.CreateTaskAsync(task);
        return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<KanbanTask>> UpdateTask(Guid id, KanbanTask task)
    {
        if (id != task.Id)
            return BadRequest();

        try
        {
            var updated = await _taskService.UpdateTaskAsync(task);
            return Ok(updated);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(Guid id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
} 
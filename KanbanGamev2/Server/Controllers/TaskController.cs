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
    public ActionResult<List<KanbanTask>> GetTasks()
    {
        var tasks = _taskService.GetTasks();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public ActionResult<KanbanTask> GetTask(Guid id)
    {
        var task = _taskService.GetTask(id);
        if (task == null)
            return NotFound();
        return Ok(task);
    }

    [HttpGet("column/{columnId}")]
    public ActionResult<List<KanbanTask>> GetTasksByColumn(string columnId)
    {
        var tasks = _taskService.GetTasksByColumn(columnId);
        return Ok(tasks);
    }

    [HttpPost]
    public ActionResult<KanbanTask> CreateTask(KanbanTask task)
    {
        var created = _taskService.CreateTask(task);
        return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<KanbanTask> UpdateTask(Guid id, KanbanTask task)
    {
        if (id != task.Id)
            return BadRequest();

        try
        {
            var updated = _taskService.UpdateTask(task);
            return Ok(updated);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteTask(Guid id)
    {
        var deleted = _taskService.DeleteTask(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
} 
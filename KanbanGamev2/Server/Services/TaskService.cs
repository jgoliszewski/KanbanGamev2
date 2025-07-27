using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class TaskService : ITaskService
{
    private List<KanbanTask> _tasks = new();

    public TaskService()
    {
        SeedData();
    }

    public List<KanbanTask> GetTasks()
    {
        return _tasks;
    }

    public KanbanTask? GetTask(Guid id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public KanbanTask CreateTask(KanbanTask task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.Now;
        _tasks.Add(task);
        return task;
    }

    public KanbanTask UpdateTask(KanbanTask task)
    {
        var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existing != null)
        {
            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Priority = task.Priority;
            existing.Status = task.Status;
            existing.AssignedToEmployeeId = task.AssignedToEmployeeId;
            existing.DueDate = task.DueDate;
            existing.LaborIntensity = task.LaborIntensity;
            existing.LaborLeft = task.LaborLeft;
            existing.ColumnId = task.ColumnId;
            existing.Order = task.Order;
            existing.UpdatedAt = DateTime.Now;
            return existing;
        }
        throw new ArgumentException("Task not found");
    }

    public bool DeleteTask(Guid id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            _tasks.Remove(task);
            return true;
        }
        return false;
    }

    public List<KanbanTask> GetTasksByColumn(string columnId)
    {
        return _tasks.Where(t => t.ColumnId == columnId).OrderBy(t => t.Order).ToList();
    }

    private void SeedData()
    {
        _tasks = new List<KanbanTask>();
        // Tasks will be generated from features when they are sent to development
    }

    public void ResetData()
    {
        _tasks.Clear();
        SeedData();
    }

    public async Task<bool> AreAllTasksCompleted(List<Guid> taskIds)
    {
        if (!taskIds.Any()) return true;

        var tasks = _tasks.Where(t => taskIds.Contains(t.Id)).ToList();
        return tasks.All(t => t.IsCompleted);
    }

    public async Task DeleteTasks(List<Guid> taskIds)
    {
        var tasksToDelete = _tasks.Where(t => taskIds.Contains(t.Id)).ToList();
        foreach (var task in tasksToDelete)
        {
            _tasks.Remove(task);
        }
        await Task.CompletedTask;
    }

    public async Task UpdateTasks()
    {
        // This method is called after work simulation to persist changes
        // In a real application, this would save to a database
        await Task.CompletedTask;
    }
}
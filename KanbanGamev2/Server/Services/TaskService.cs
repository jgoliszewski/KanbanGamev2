using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class TaskService : ITaskService
{
    private List<KanbanTask> _tasks = new();

    public TaskService()
    {
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
            // Check if task was moved to a done column and update dependencies
            if (IsDoneColumn(task.ColumnId) && !IsDoneColumn(existing.ColumnId))
            {
                UpdateDependentTasks(existing.Id);
            }

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
            existing.DependsOnTaskId = task.DependsOnTaskId;
            existing.FeatureId = task.FeatureId;
            existing.BoardType = task.BoardType;
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



    public void ResetData()
    {
        _tasks.Clear();
    }

    private bool IsDoneColumn(string columnId)
    {
        return columnId?.Contains("done") == true;
    }

    private void UpdateDependentTasks(Guid completedTaskId)
    {
        // Find all tasks that depend on the completed task
        var dependentTasks = _tasks.Where(t => t.DependsOnTaskId == completedTaskId).ToList();

        foreach (var dependentTask in dependentTasks)
        {
            // Set the dependency as completed
            dependentTask.IsDependencyCompleted = true;
            dependentTask.UpdatedAt = DateTime.Now;

            Console.WriteLine($"Updated task '{dependentTask.Title}' - dependency on completed task is now satisfied");
        }
    }
}
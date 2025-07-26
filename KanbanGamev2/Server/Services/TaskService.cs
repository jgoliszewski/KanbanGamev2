using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class TaskService : ITaskService
{
    private List<KanbanTask> _tasks = new();

    public TaskService()
    {
        SeedData();
    }

    public async Task<List<KanbanTask>> GetTasksAsync()
    {
        return await Task.FromResult(_tasks);
    }

    public async Task<KanbanTask?> GetTaskAsync(Guid id)
    {
        return await Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
    }

    public async Task<KanbanTask> CreateTaskAsync(KanbanTask task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.Now;
        _tasks.Add(task);
        return await Task.FromResult(task);
    }

    public async Task<KanbanTask> UpdateTaskAsync(KanbanTask task)
    {
        var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existing != null)
        {
            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Priority = task.Priority;
            existing.Status = task.Status;
            existing.AssignedTo = task.AssignedTo;
            existing.DueDate = task.DueDate;
            existing.EstimatedHours = task.EstimatedHours;
            existing.ActualHours = task.ActualHours;
            existing.ColumnId = task.ColumnId;
            existing.Order = task.Order;
            existing.UpdatedAt = DateTime.Now;
            return await Task.FromResult(existing);
        }
        throw new ArgumentException("Task not found");
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            _tasks.Remove(task);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<List<KanbanTask>> GetTasksByColumnAsync(string columnId)
    {
        return await Task.FromResult(_tasks.Where(t => t.ColumnId == columnId).OrderBy(t => t.Order).ToList());
    }

    private void SeedData()
    {
        _tasks = new List<KanbanTask>
        {
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Fix Login Bug",
                Description = "Users cannot log in with correct credentials",
                Priority = Priority.High,
                Status = Status.InProgress,
                AssignedTo = "John Doe",
                DueDate = DateTime.Now.AddDays(2),
                EstimatedHours = 4,
                ActualHours = 2,
                ColumnId = "backend-dev-doing",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Update Documentation",
                Description = "Update API documentation with new endpoints",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedTo = "",
                DueDate = DateTime.Now.AddDays(5),
                EstimatedHours = 6,
                ActualHours = 0,
                ColumnId = "backend-backlog",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Code Review",
                Description = "Review pull request #123 for user management feature",
                Priority = Priority.Low,
                Status = Status.Done,
                AssignedTo = "Jane Smith",
                DueDate = DateTime.Now.AddDays(-1),
                EstimatedHours = 2,
                ActualHours = 1,
                ColumnId = "backend-done",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Design Homepage",
                Description = "Create responsive homepage design",
                Priority = Priority.High,
                Status = Status.InProgress,
                AssignedTo = "David Brown",
                DueDate = DateTime.Now.AddDays(3),
                EstimatedHours = 8,
                ActualHours = 4,
                ColumnId = "frontend-dev-doing",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Implement Search",
                Description = "Add search functionality to the application",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedTo = "",
                DueDate = DateTime.Now.AddDays(7),
                EstimatedHours = 12,
                ActualHours = 0,
                ColumnId = "frontend-backlog",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Unit Tests",
                Description = "Write unit tests for authentication module",
                Priority = Priority.Medium,
                Status = Status.InProgress,
                AssignedTo = "Sarah Wilson",
                DueDate = DateTime.Now.AddDays(4),
                EstimatedHours = 6,
                ActualHours = 3,
                ColumnId = "backend-test-doing",
                Order = 1
            }
        };
    }

    public void ResetData()
    {
        _tasks.Clear();
        SeedData();
    }
} 
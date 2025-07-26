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
            existing.EstimatedHours = task.EstimatedHours;
            existing.ActualHours = task.ActualHours;
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
        _tasks = new List<KanbanTask>
        {
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Setup Database Schema",
                Description = "Create initial database tables and relationships",
                Priority = Priority.High,
                Status = Status.InProgress,
                AssignedToEmployeeId = null, // Will be assigned via drag and drop
                DueDate = DateTime.Now.AddDays(3),
                EstimatedHours = 8,
                ActualHours = 4,
                ColumnId = "backend-dev-doing",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Implement API Endpoints",
                Description = "Create REST API endpoints for user management",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(5),
                EstimatedHours = 12,
                ActualHours = 0,
                ColumnId = "backend-dev-waiting",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Design User Interface",
                Description = "Create wireframes and mockups for the main dashboard",
                Priority = Priority.Medium,
                Status = Status.InProgress,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(7),
                EstimatedHours = 6,
                ActualHours = 3,
                ColumnId = "frontend-dev-doing",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Write Unit Tests",
                Description = "Create comprehensive unit tests for backend services",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(4),
                EstimatedHours = 10,
                ActualHours = 0,
                ColumnId = "backend-test-waiting",
                Order = 1
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Performance Testing",
                Description = "Conduct load testing and performance optimization",
                Priority = Priority.Low,
                Status = Status.Done,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(-1),
                EstimatedHours = 4,
                ActualHours = 4,
                ColumnId = "backend-done",
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
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
        _tasks = new List<KanbanTask>
        {
            // All tasks start in backlog
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Mobile App - API Design",
                Description = "Design REST API endpoints for mobile application",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(15),
                StoryPoints = 3,
                ColumnId = "backlog",
                Order = 1,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "File Upload System - Backend Implementation",
                Description = "Implement backend logic for file upload system",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(5),
                StoryPoints = 4,
                ColumnId = "backlog",
                Order = 2,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Search Functionality - Database Schema",
                Description = "Create database schema for search functionality",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(3),
                StoryPoints = 2,
                ColumnId = "backlog",
                Order = 3,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            
            // Frontend tasks in backlog
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Real-time Chat - Frontend Analysis",
                Description = "Analyze frontend requirements for real-time chat",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(8),
                StoryPoints = 3,
                ColumnId = "backlog",
                Order = 4,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Email Notifications - UI Components",
                Description = "Create UI components for email notification system",
                Priority = Priority.Low,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(3),
                StoryPoints = 2,
                ColumnId = "backlog",
                Order = 5,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            
            // More tasks in backlog
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Mobile App - Backend Implementation",
                Description = "Implement backend logic for mobile application",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(20),
                StoryPoints = 8,
                ColumnId = "backlog",
                Order = 6,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            }
        };
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
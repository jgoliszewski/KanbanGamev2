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
            // Backend Backlog Tasks
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Setup Database Schema",
                Description = "Create initial database tables and relationships",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(3),
                EstimatedHours = 8,
                ActualHours = 0,
                ColumnId = "backend-backlog",
                Order = 1,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
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
                ColumnId = "backend-backlog",
                Order = 2,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Database Migration Scripts",
                Description = "Create migration scripts for database versioning",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(4),
                EstimatedHours = 6,
                ActualHours = 0,
                ColumnId = "backend-backlog",
                Order = 3,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Authentication Service",
                Description = "Implement JWT-based authentication system",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(6),
                EstimatedHours = 10,
                ActualHours = 0,
                ColumnId = "backend-backlog",
                Order = 4,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Logging Infrastructure",
                Description = "Setup centralized logging and monitoring",
                Priority = Priority.Low,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(8),
                EstimatedHours = 4,
                ActualHours = 0,
                ColumnId = "backend-backlog",
                Order = 5,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            
            // Frontend Backlog Tasks
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Design User Interface",
                Description = "Create wireframes and mockups for the main dashboard",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(7),
                EstimatedHours = 6,
                ActualHours = 0,
                ColumnId = "frontend-backlog",
                Order = 1,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Responsive Layout",
                Description = "Implement responsive design for mobile devices",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(5),
                EstimatedHours = 8,
                ActualHours = 0,
                ColumnId = "frontend-backlog",
                Order = 2,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "State Management",
                Description = "Setup Redux store and state management",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(4),
                EstimatedHours = 10,
                ActualHours = 0,
                ColumnId = "frontend-backlog",
                Order = 3,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Component Library",
                Description = "Create reusable UI components",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(6),
                EstimatedHours = 12,
                ActualHours = 0,
                ColumnId = "frontend-backlog",
                Order = 4,
                LaborIntensity = 1.0,
                LaborLeft = 1.0
            },
            new KanbanTask
            {
                Id = Guid.NewGuid(),
                Title = "Form Validation",
                Description = "Implement client-side form validation",
                Priority = Priority.Low,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(3),
                EstimatedHours = 4,
                ActualHours = 0,
                ColumnId = "frontend-backlog",
                Order = 5,
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

    public async Task UpdateTasks()
    {
        // This method is called after work simulation to persist changes
        // In a real application, this would save to a database
        await Task.CompletedTask;
    }
} 
using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class FeatureService : IFeatureService
{
    private List<Feature> _features = new();
    private readonly ITaskService _taskService;

    public FeatureService(ITaskService taskService)
    {
        _taskService = taskService;
        SeedData();
    }

    public List<Feature> GetFeatures()
    {
        return _features;
    }

    public Feature? GetFeature(Guid id)
    {
        return _features.FirstOrDefault(f => f.Id == id);
    }

    public Feature CreateFeature(Feature feature)
    {
        feature.Id = Guid.NewGuid();
        feature.CreatedAt = DateTime.Now;
        _features.Add(feature);
        return feature;
    }

    public Feature UpdateFeature(Feature feature)
    {
        var existing = _features.FirstOrDefault(f => f.Id == feature.Id);
        if (existing != null)
        {
            existing.Title = feature.Title;
            existing.Description = feature.Description;
            existing.Priority = feature.Priority;
            existing.Status = feature.Status;
            existing.AssignedToEmployeeId = feature.AssignedToEmployeeId;
            existing.DueDate = feature.DueDate;
            existing.StoryPoints = feature.StoryPoints;
            existing.LaborIntensity = feature.LaborIntensity;
            existing.LaborLeft = feature.LaborLeft;
            existing.ColumnId = feature.ColumnId;
            existing.Order = feature.Order;
            existing.Profit = feature.Profit;
            existing.GeneratedTaskIds = feature.GeneratedTaskIds;
            existing.UpdatedAt = DateTime.Now;
            return existing;
        }
        throw new ArgumentException("Feature not found");
    }

    public bool DeleteFeature(Guid id)
    {
        var feature = _features.FirstOrDefault(f => f.Id == id);
        if (feature != null)
        {
            _features.Remove(feature);
            return true;
        }
        return false;
    }

    public List<Feature> GetFeaturesByColumn(string columnId)
    {
        return _features.Where(f => f.ColumnId == columnId).OrderBy(f => f.Order).ToList();
    }

    public async Task SendFeatureToDevelopment(Feature feature)
    {
        // Move feature to development column in summary board
        feature.ColumnId = "development";
        feature.Status = Status.InProgress;
        UpdateFeature(feature);

        // Create frontend tasks
        var frontendTasks = CreateFrontendTasks(feature);
        foreach (var task in frontendTasks)
        {
            var createdTask = _taskService.CreateTask(task);
            feature.GeneratedTaskIds.Add(createdTask.Id);
        }

        // Create backend tasks
        var backendTasks = CreateBackendTasks(feature);
        foreach (var task in backendTasks)
        {
            var createdTask = _taskService.CreateTask(task);
            feature.GeneratedTaskIds.Add(createdTask.Id);
        }

        // Update feature with task references
        UpdateFeature(feature);
    }

    private List<KanbanTask> CreateFrontendTasks(Feature feature)
    {
        var tasks = new List<KanbanTask>();
        var baseStoryPoints = feature.StoryPoints / 2; // Split between frontend and backend

        // UI Design task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - UI Design",
            Description = $"Design user interface for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / 3),
            ColumnId = "frontend-backlog",
            Order = 1,
            LaborIntensity = 1.0,
            LaborLeft = 1.0
        });

        // Component Development task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Component Development",
            Description = $"Develop React components for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(3, baseStoryPoints / 2),
            ColumnId = "frontend-backlog",
            Order = 2,
            LaborIntensity = 1.0,
            LaborLeft = 1.0
        });

        // Integration task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Frontend Integration",
            Description = $"Integrate {feature.Title} with backend APIs",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / 3),
            ColumnId = "frontend-backlog",
            Order = 3,
            LaborIntensity = 1.0,
            LaborLeft = 1.0
        });

        return tasks;
    }

    private List<KanbanTask> CreateBackendTasks(Feature feature)
    {
        var tasks = new List<KanbanTask>();
        var baseStoryPoints = feature.StoryPoints / 2; // Split between frontend and backend

        // API Design task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - API Design",
            Description = $"Design REST API endpoints for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / 3),
            ColumnId = "backend-backlog",
            Order = 1,
            LaborIntensity = 1.0,
            LaborLeft = 1.0
        });

        // Backend Implementation task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Backend Implementation",
            Description = $"Implement backend logic for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(4, baseStoryPoints),
            ColumnId = "backend-backlog",
            Order = 2,
            LaborIntensity = 1.0,
            LaborLeft = 1.0
        });

        // Database task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Database Schema",
            Description = $"Create database schema for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / 3),
            ColumnId = "backend-backlog",
            Order = 3,
            LaborIntensity = 1.0,
            LaborLeft = 1.0
        });

        return tasks;
    }

    private void SeedData()
    {
        _features = new List<Feature>
        {
            // Analysis Backlog Features
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "User Authentication",
                Description = "Implement secure user authentication system",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(7),
                StoryPoints = 8,
                ColumnId = "backlog",
                Order = 1,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 15000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Dashboard Analytics",
                Description = "Create analytics dashboard with charts and metrics",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(14),
                StoryPoints = 13,
                ColumnId = "backlog",
                Order = 2,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 25000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Payment Integration",
                Description = "Integrate payment gateway for subscription management",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(10),
                StoryPoints = 21,
                ColumnId = "backlog",
                Order = 3,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 35000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Mobile App",
                Description = "Develop mobile application for iOS and Android",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(30),
                StoryPoints = 34,
                ColumnId = "backlog",
                Order = 4,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 50000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Real-time Chat",
                Description = "Implement real-time messaging system",
                Priority = Priority.High,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(12),
                StoryPoints = 13,
                ColumnId = "backlog",
                Order = 5,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 30000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "File Upload System",
                Description = "Create secure file upload and storage system",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(8),
                StoryPoints = 8,
                ColumnId = "backlog",
                Order = 6,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 20000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Email Notifications",
                Description = "Add email notification system for important events",
                Priority = Priority.Low,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(5),
                StoryPoints = 5,
                ColumnId = "backlog",
                Order = 7,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 12000
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Search Functionality",
                Description = "Implement advanced search with filters",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(15),
                StoryPoints = 13,
                ColumnId = "backlog",
                Order = 8,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 28000
            }
        };
    }

    public void ResetData()
    {
        _features.Clear();
        SeedData();
    }

    public async Task UpdateFeatures()
    {
        // This method is called after work simulation to persist changes
        // In a real application, this would save to a database
        await Task.CompletedTask;
    }
} 
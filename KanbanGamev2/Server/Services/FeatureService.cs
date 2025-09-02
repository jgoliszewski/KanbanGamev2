using KanbanGame.Shared;
using Microsoft.AspNetCore.SignalR;
using KanbanGamev2.Server.SignalR;

namespace KanbanGamev2.Server.Services;

public class FeatureService : IFeatureService
{
    private List<Feature> _features = new();
    private readonly ITaskService _taskService;
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly INotificationService _notificationService;

    public FeatureService(ITaskService taskService, IHubContext<GameHub> gameHubContext, INotificationService notificationService)
    {
        _taskService = taskService;
        _gameHubContext = gameHubContext;
        _notificationService = notificationService;
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

        // Create frontend tasks first (without dependencies)
        var frontendTasks = CreateFrontendTasks(feature);
        var createdFrontendTasks = new List<KanbanTask>();
        
        foreach (var task in frontendTasks)
        {
            var createdTask = _taskService.CreateTask(task);
            createdFrontendTasks.Add(createdTask);
            feature.GeneratedTaskIds.Add(createdTask.Id);
        }

        // Create backend tasks first (without dependencies)
        var backendTasks = CreateBackendTasks(feature);
        var createdBackendTasks = new List<KanbanTask>();
        
        foreach (var task in backendTasks)
        {
            var createdTask = _taskService.CreateTask(task);
            createdBackendTasks.Add(createdTask);
            feature.GeneratedTaskIds.Add(createdTask.Id);
        }

        // Now update the dependencies with the actual created task IDs
        UpdateTaskDependencies(createdFrontendTasks, createdBackendTasks);

        // Update feature with task references
        UpdateFeature(feature);

        // Send global notification about feature being sent to development
        await _notificationService.SendGlobalNotificationAsync("Feature Sent to Development",
            $"Feature '{feature.Title}' has been sent to development. {feature.GeneratedTaskIds.Count} tasks have been created.",
            "Info");

        // Notify all connected users to refresh their boards
        await _gameHubContext.Clients.All.SendAsync("RefreshAllBoards");
    }

    private void UpdateTaskDependencies(List<KanbanTask> frontendTasks, List<KanbanTask> backendTasks)
    {
        // Update frontend task dependencies - every third task depends on the previous one
        if (frontendTasks.Count >= 3)
        {
            for (int i = 2; i < frontendTasks.Count; i += 3) // Start from index 2 (third task), increment by 3
            {
                var dependentTask = frontendTasks[i];
                var dependencyTask = frontendTasks[i - 1]; // Depends on the previous task
                dependentTask.DependsOnTaskId = dependencyTask.Id;
                _taskService.UpdateTask(dependentTask);
            }
        }

        // Update backend task dependencies - every third task depends on the previous one
        if (backendTasks.Count >= 3)
        {
            for (int i = 2; i < backendTasks.Count; i += 3) // Start from index 2 (third task), increment by 3
            {
                var dependentTask = backendTasks[i];
                var dependencyTask = backendTasks[i - 1]; // Depends on the previous task
                dependentTask.DependsOnTaskId = dependencyTask.Id;
                _taskService.UpdateTask(dependentTask);
            }
        }
    }

    private List<KanbanTask> CreateFrontendTasks(Feature feature)
    {
        var tasks = new List<KanbanTask>();
        var baseStoryPoints = feature.StoryPoints / 2; // Split between frontend and backend
        
        // Determine number of frontend tasks (4-8 based on story points)
        var numFrontendTasks = Math.Min(8, Math.Max(4, feature.StoryPoints / 2));

        // UI Design task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - UI Design",
            Description = $"Design user interface for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
            ColumnId = "frontend-backlog",
            Order = 1,
            LaborIntensity = 1.0,
            LaborLeft = 1.0,
            DependsOnTaskId = null,
            FeatureId = feature.Id,
            BoardType = BoardType.Frontend
        });

        // Component Development task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Component Development",
            Description = $"Develop React components for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(3, baseStoryPoints / numFrontendTasks),
            ColumnId = "frontend-backlog",
            Order = 2,
            LaborIntensity = 1.0,
            LaborLeft = 1.0,
            DependsOnTaskId = null, // Will be updated after creation
            FeatureId = feature.Id,
            BoardType = BoardType.Frontend
        });

        // State Management task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - State Management",
            Description = $"Implement state management for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
            ColumnId = "frontend-backlog",
            Order = 3,
            LaborIntensity = 1.0,
            LaborLeft = 1.0,
            DependsOnTaskId = null, // Will be updated after creation
            FeatureId = feature.Id,
            BoardType = BoardType.Frontend
        });

        // Routing and Navigation task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Routing and Navigation",
            Description = $"Set up routing and navigation for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
            ColumnId = "frontend-backlog",
            Order = 4,
            LaborIntensity = 1.0,
            LaborLeft = 1.0,
            DependsOnTaskId = null, // Will be updated after creation
            FeatureId = feature.Id,
            BoardType = BoardType.Frontend
        });

        // Add more tasks if needed for larger features
        if (numFrontendTasks >= 5)
        {
            // Form Handling task
            tasks.Add(new KanbanTask
            {
                Title = $"{feature.Title} - Form Handling",
                Description = $"Implement form handling and validation for {feature.Title}",
                Priority = feature.Priority,
                Status = Status.ToDo,
                StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
                ColumnId = "frontend-backlog",
                Order = 5,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                DependsOnTaskId = null,
                FeatureId = feature.Id,
                BoardType = BoardType.Frontend
            });
        }

        if (numFrontendTasks >= 6)
        {
            // Error Handling task
            tasks.Add(new KanbanTask
            {
                Title = $"{feature.Title} - Error Handling",
                Description = $"Implement error handling and user feedback for {feature.Title}",
                Priority = feature.Priority,
                Status = Status.ToDo,
                StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
                ColumnId = "frontend-backlog",
                Order = 6,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                DependsOnTaskId = null,
                FeatureId = feature.Id,
                BoardType = BoardType.Frontend
            });
        }

        if (numFrontendTasks >= 7)
        {
            // Performance Optimization task
            tasks.Add(new KanbanTask
            {
                Title = $"{feature.Title} - Performance Optimization",
                Description = $"Optimize performance and loading times for {feature.Title}",
                Priority = feature.Priority,
                Status = Status.ToDo,
                StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
                ColumnId = "frontend-backlog",
                Order = 7,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                DependsOnTaskId = null,
                FeatureId = feature.Id,
                BoardType = BoardType.Frontend
            });
        }

        if (numFrontendTasks >= 8)
        {
            // Frontend Integration task
            tasks.Add(new KanbanTask
            {
                Title = $"{feature.Title} - Frontend Integration",
                Description = $"Final integration and testing for {feature.Title}",
                Priority = feature.Priority,
                Status = Status.ToDo,
                StoryPoints = Math.Max(2, baseStoryPoints / numFrontendTasks),
                ColumnId = "frontend-backlog",
                Order = 8,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                DependsOnTaskId = null, // Will be updated after creation
                FeatureId = feature.Id,
                BoardType = BoardType.Frontend
            });
        }

        return tasks;
    }

    private List<KanbanTask> CreateBackendTasks(Feature feature)
    {
        var tasks = new List<KanbanTask>();
        var baseStoryPoints = feature.StoryPoints / 2; // Split between frontend and backend
        
        // Determine number of backend tasks (2-4 based on story points)
        var numBackendTasks = Math.Min(4, Math.Max(2, feature.StoryPoints / 4));

        // API Design task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - API Design",
            Description = $"Design REST API endpoints for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / numBackendTasks),
            ColumnId = "backend-backlog",
            Order = 1,
            LaborIntensity = 1.0,
            LaborLeft = 1.0,
            DependsOnTaskId = null,
            FeatureId = feature.Id,
            BoardType = BoardType.Backend
        });

        // Database Schema task
        tasks.Add(new KanbanTask
        {
            Title = $"{feature.Title} - Database Schema",
            Description = $"Create database schema for {feature.Title}",
            Priority = feature.Priority,
            Status = Status.ToDo,
            StoryPoints = Math.Max(2, baseStoryPoints / numBackendTasks),
            ColumnId = "backend-backlog",
            Order = 2,
            LaborIntensity = 1.0,
            LaborLeft = 1.0,
            DependsOnTaskId = null, // Will be updated after creation
            FeatureId = feature.Id,
            BoardType = BoardType.Backend
        });

        // Add more tasks if needed for larger features
        if (numBackendTasks >= 3)
        {
            // Backend Implementation task
            tasks.Add(new KanbanTask
            {
                Title = $"{feature.Title} - Backend Implementation",
                Description = $"Implement core backend logic for {feature.Title}",
                Priority = feature.Priority,
                Status = Status.ToDo,
                StoryPoints = Math.Max(3, baseStoryPoints / numBackendTasks),
                ColumnId = "backend-backlog",
                Order = 3,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                DependsOnTaskId = null, // Will be updated after creation
                FeatureId = feature.Id,
                BoardType = BoardType.Backend
            });
        }

        if (numBackendTasks >= 4)
        {
            // Testing and Validation task
            tasks.Add(new KanbanTask
            {
                Title = $"{feature.Title} - Testing and Validation",
                Description = $"Implement testing and validation for {feature.Title}",
                Priority = feature.Priority,
                Status = Status.ToDo,
                StoryPoints = Math.Max(2, baseStoryPoints / numBackendTasks),
                ColumnId = "backend-backlog",
                Order = 4,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                DependsOnTaskId = null, // Will be updated after creation
                FeatureId = feature.Id,
                BoardType = BoardType.Backend
            });
        }

        return tasks;
    }

    private void SeedData()
    {
        _features = new List<Feature>
        {
            // Ready for Development Features
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "User Authentication",
                Description = "Implement secure user authentication system",
                Priority = Priority.High,
                Status = Status.Waiting,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(7),
                StoryPoints = 8,
                ColumnId = "ready-dev",
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
                Status = Status.Waiting,
                AssignedToEmployeeId = null,
                DueDate = DateTime.Now.AddDays(14),
                StoryPoints = 13,
                ColumnId = "ready-dev",
                Order = 2,
                LaborIntensity = 1.0,
                LaborLeft = 1.0,
                Profit = 25000
            },
            // Analysis Backlog Features
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
}
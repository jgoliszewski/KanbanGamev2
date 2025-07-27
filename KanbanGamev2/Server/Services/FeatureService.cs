using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class FeatureService : IFeatureService
{
    private List<Feature> _features = new();

    public FeatureService()
    {
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
                LaborLeft = 1.0
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
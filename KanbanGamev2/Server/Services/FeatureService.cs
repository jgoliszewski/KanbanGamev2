using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public class FeatureService : IFeatureService
{
    private List<Feature> _features = new();

    public FeatureService()
    {
        SeedData();
    }

    public async Task<List<Feature>> GetFeaturesAsync()
    {
        return await Task.FromResult(_features);
    }

    public async Task<Feature?> GetFeatureAsync(Guid id)
    {
        return await Task.FromResult(_features.FirstOrDefault(f => f.Id == id));
    }

    public async Task<Feature> CreateFeatureAsync(Feature feature)
    {
        feature.Id = Guid.NewGuid();
        feature.CreatedAt = DateTime.Now;
        _features.Add(feature);
        return await Task.FromResult(feature);
    }

    public async Task<Feature> UpdateFeatureAsync(Feature feature)
    {
        var existing = _features.FirstOrDefault(f => f.Id == feature.Id);
        if (existing != null)
        {
            existing.Title = feature.Title;
            existing.Description = feature.Description;
            existing.Priority = feature.Priority;
            existing.Status = feature.Status;
            existing.AssignedTo = feature.AssignedTo;
            existing.DueDate = feature.DueDate;
            existing.StoryPoints = feature.StoryPoints;
            existing.ColumnId = feature.ColumnId;
            existing.Order = feature.Order;
            existing.UpdatedAt = DateTime.Now;
            return await Task.FromResult(existing);
        }
        throw new ArgumentException("Feature not found");
    }

    public async Task<bool> DeleteFeatureAsync(Guid id)
    {
        var feature = _features.FirstOrDefault(f => f.Id == id);
        if (feature != null)
        {
            _features.Remove(feature);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<List<Feature>> GetFeaturesByColumnAsync(string columnId)
    {
        return await Task.FromResult(_features.Where(f => f.ColumnId == columnId).OrderBy(f => f.Order).ToList());
    }

    private void SeedData()
    {
        _features = new List<Feature>
        {
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "User Authentication",
                Description = "Implement secure user authentication system",
                Priority = Priority.High,
                Status = Status.InProgress,
                AssignedTo = "John Doe",
                DueDate = DateTime.Now.AddDays(7),
                StoryPoints = 8,
                ColumnId = "analysis1",
                Order = 1
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Dashboard Analytics",
                Description = "Create analytics dashboard with charts and metrics",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedTo = "",
                DueDate = DateTime.Now.AddDays(14),
                StoryPoints = 13,
                ColumnId = "backlog",
                Order = 1
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Email Notifications",
                Description = "Add email notification system for important events",
                Priority = Priority.Low,
                Status = Status.Done,
                AssignedTo = "Jane Smith",
                DueDate = DateTime.Now.AddDays(-2),
                StoryPoints = 5,
                ColumnId = "done",
                Order = 1
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Payment Integration",
                Description = "Integrate payment gateway for subscription management",
                Priority = Priority.High,
                Status = Status.InProgress,
                AssignedTo = "Mike Johnson",
                DueDate = DateTime.Now.AddDays(10),
                StoryPoints = 21,
                ColumnId = "development",
                Order = 1
            },
            new Feature
            {
                Id = Guid.NewGuid(),
                Title = "Mobile App",
                Description = "Develop mobile application for iOS and Android",
                Priority = Priority.Medium,
                Status = Status.ToDo,
                AssignedTo = "",
                DueDate = DateTime.Now.AddDays(30),
                StoryPoints = 34,
                ColumnId = "backlog",
                Order = 2
            }
        };
    }

    public void ResetData()
    {
        _features.Clear();
        SeedData();
    }
} 
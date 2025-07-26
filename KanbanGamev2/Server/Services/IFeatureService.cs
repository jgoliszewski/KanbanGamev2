using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface IFeatureService
{
    Task<List<Feature>> GetFeaturesAsync();
    Task<Feature?> GetFeatureAsync(Guid id);
    Task<Feature> CreateFeatureAsync(Feature feature);
    Task<Feature> UpdateFeatureAsync(Feature feature);
    Task<bool> DeleteFeatureAsync(Guid id);
    Task<List<Feature>> GetFeaturesByColumnAsync(string columnId);
} 
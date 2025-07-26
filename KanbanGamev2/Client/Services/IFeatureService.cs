using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface IFeatureService
{
    List<Feature> Features { get; set; }
    Task GetFeatures();
    Task<Feature?> GetFeature(Guid id);
    Task<Feature> CreateFeature(Feature feature);
    Task<Feature> UpdateFeature(Feature feature);
    Task<bool> DeleteFeature(Guid id);
    Task<List<Feature>> GetFeaturesByColumn(string columnId);
} 
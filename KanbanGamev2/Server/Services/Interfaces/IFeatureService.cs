using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface IFeatureService
{
    List<Feature> GetFeatures();
    Feature? GetFeature(Guid id);
    Task<Feature> CreateFeature(Feature feature);
    Feature UpdateFeature(Feature feature);
    Task<bool> DeleteFeature(Guid id);
    List<Feature> GetFeaturesByColumn(string columnId);
    void ResetData();
    Task SendFeatureToDevelopment(Feature feature);
} 
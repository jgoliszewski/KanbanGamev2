using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface IFeatureService
{
    List<Feature> Features { get; set; }
    Task GetFeatures();
    Task<Feature> UpdateFeature(Feature feature);
    Task SendFeatureToDevelopment(Feature feature);
}
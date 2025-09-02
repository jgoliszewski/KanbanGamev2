using KanbanGame.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace KanbanGamev2.Client.Services;

public class FeatureService : IFeatureService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _navigationManager;
    private readonly ITaskService _taskService;
    private readonly ISignalRService _signalRService;

    public List<Feature> Features { get; set; } = new();

    public FeatureService(HttpClient http, NavigationManager navigationManager, ITaskService taskService, ISignalRService signalRService)
    {
        _http = http;
        _navigationManager = navigationManager;
        _taskService = taskService;
        _signalRService = signalRService;
    }

    public async Task GetFeatures()
    {
        var result = await _http.GetFromJsonAsync<List<Feature>>("api/feature");
        if (result is not null)
            Features = result;
    }

    public async Task<Feature> UpdateFeature(Feature feature)
    {
        var response = await _http.PutAsJsonAsync($"api/feature/{feature.Id}", feature);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Feature>() ?? feature;
    }

    public async Task SendFeatureToDevelopment(Feature feature)
    {
        // Call the API endpoint to send feature to development
        var response = await _http.PostAsync($"api/feature/{feature.Id}/send-to-development", null);
        response.EnsureSuccessStatusCode();

        // Refresh features and tasks
        await GetFeatures();
        await _taskService.GetTasks();

        Console.WriteLine($"Feature '{feature.Title}' sent to development and boards refreshed");
    }
}
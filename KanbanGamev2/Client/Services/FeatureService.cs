using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public class FeatureService : IFeatureService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _navigationManager;

    public List<Feature> Features { get; set; } = new();

    public FeatureService(HttpClient http, NavigationManager navigationManager)
    {
        _http = http;
        _navigationManager = navigationManager;
    }

    public async Task GetFeatures()
    {
        var result = await _http.GetFromJsonAsync<List<Feature>>("api/feature");
        if (result is not null)
            Features = result;
    }

    public async Task<Feature?> GetFeature(Guid id)
    {
        return await _http.GetFromJsonAsync<Feature>($"api/feature/{id}");
    }

    public async Task<Feature> CreateFeature(Feature feature)
    {
        var response = await _http.PostAsJsonAsync("api/feature", feature);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Feature>() ?? feature;
    }

    public async Task<Feature> UpdateFeature(Feature feature)
    {
        var response = await _http.PutAsJsonAsync($"api/feature/{feature.Id}", feature);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Feature>() ?? feature;
    }

    public async Task<bool> DeleteFeature(Guid id)
    {
        var response = await _http.DeleteAsync($"api/feature/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Feature>> GetFeaturesByColumn(string columnId)
    {
        var result = await _http.GetFromJsonAsync<List<Feature>>($"api/feature/column/{columnId}");
        return result ?? new List<Feature>();
    }

    public async Task UpdateFeatures()
    {
        // This method is called after work simulation to persist changes
        await Task.CompletedTask;
    }
} 
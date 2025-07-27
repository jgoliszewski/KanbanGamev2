using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public class TaskService : ITaskService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _navigationManager;

    public List<KanbanTask> Tasks { get; set; } = new();

    public TaskService(HttpClient http, NavigationManager navigationManager)
    {
        _http = http;
        _navigationManager = navigationManager;
    }

    public async Task GetTasks()
    {
        var result = await _http.GetFromJsonAsync<List<KanbanTask>>("api/task");
        if (result is not null)
            Tasks = result;
    }

    public async Task<KanbanTask?> GetTask(Guid id)
    {
        return await _http.GetFromJsonAsync<KanbanTask>($"api/task/{id}");
    }

    public async Task<KanbanTask> CreateTask(KanbanTask task)
    {
        var response = await _http.PostAsJsonAsync("api/task", task);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<KanbanTask>() ?? task;
    }

    public async Task<KanbanTask> UpdateTask(KanbanTask task)
    {
        var response = await _http.PutAsJsonAsync($"api/task/{task.Id}", task);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<KanbanTask>() ?? task;
    }

    public async Task<bool> DeleteTask(Guid id)
    {
        var response = await _http.DeleteAsync($"api/task/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<KanbanTask>> GetTasksByColumn(string columnId)
    {
        var result = await _http.GetFromJsonAsync<List<KanbanTask>>($"api/task/column/{columnId}");
        return result ?? new List<KanbanTask>();
    }

    public async Task UpdateTasks()
    {
        // This method is called after work simulation to persist changes
        await Task.CompletedTask;
    }
} 
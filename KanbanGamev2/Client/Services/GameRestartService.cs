using KanbanGamev2.Client.Services;
using System.Net.Http.Json;

namespace KanbanGamev2.Client.Services;

public class GameRestartService : IGameRestartService
{
    private readonly HttpClient _httpClient;
    private readonly ISignalRService _signalRService;
    private readonly IGlobalLoaderService _globalLoaderService;
    private readonly INotificationService _notificationService;

    public GameRestartService(HttpClient httpClient, ISignalRService signalRService, IGlobalLoaderService globalLoaderService, INotificationService notificationService)
    {
        _httpClient = httpClient;
        _signalRService = signalRService;
        _globalLoaderService = globalLoaderService;
        _notificationService = notificationService;
    }

    public async Task<bool> RestartGameAsync()
    {
        try
        {
            _globalLoaderService.Show("Restarting game...");
            
            var response = await _httpClient.PostAsync("api/gamestate/restart", null);
            
            if (response.IsSuccessStatusCode)
            {
                // Refresh the page to show the new state
                await _signalRService.DisconnectAsync();
                await Task.Delay(500); // Small delay to ensure disconnection
                
                // Reload the page
                var uri = new Uri(_httpClient.BaseAddress!, "");
                await Task.Delay(100); // Small delay before reload
                
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error restarting game: {ex.Message}");
            return false;
        }
        finally
        {
            _globalLoaderService.Hide();
        }
    }
} 
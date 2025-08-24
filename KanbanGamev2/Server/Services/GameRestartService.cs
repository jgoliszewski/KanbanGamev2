using KanbanGamev2.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using KanbanGamev2.Server.SignalR;

namespace KanbanGamev2.Server.Services;

public class GameRestartService : IGameRestartService
{
    private readonly IFeatureService _featureService;
    private readonly ITaskService _taskService;
    private readonly IEmployeeService _employeeService;
    private readonly IGameStateService _gameStateService;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public GameRestartService(
        IFeatureService featureService,
        ITaskService taskService,
        IEmployeeService employeeService,
        IGameStateService gameStateService,
        IHubContext<NotificationHub> notificationHub)
    {
        _featureService = featureService;
        _taskService = taskService;
        _employeeService = employeeService;
        _gameStateService = gameStateService;
        _notificationHub = notificationHub;
    }

    public async Task RestartGameAsync()
    {
        // Reset all service data
        _featureService.ResetData();
        _taskService.ResetData();
        _employeeService.ResetData();
        
        // Reset game state
        await _gameStateService.RestartGame();

        // Send notification after restart is complete
        await _notificationHub.Clients.All.SendAsync("ReceiveGlobalNotification", 
            "Game Restarted", 
            "The game has been successfully restarted. All progress has been reset to day 1 with $10,000 starting money.", 
            "Success");

        // Signal all clients to refresh their boards
        await _notificationHub.Clients.All.SendAsync("RefreshAllBoards");
    }
} 
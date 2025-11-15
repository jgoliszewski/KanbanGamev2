using KanbanGame.Shared;
using KanbanGamev2.Server.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace KanbanGamev2.Server.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<GameHub> _gameHub;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public NotificationService(IHubContext<GameHub> gameHub, IHubContext<NotificationHub> notificationHub)
    {
        _gameHub = gameHub;
        _notificationHub = notificationHub;
    }

    public async Task SendGlobalNotificationAsync(string title, string message, string type)
    {
        await _notificationHub.Clients.All.SendAsync("ReceiveGlobalNotification", title, message, type);
    }

    public async Task NotifyEmployeeStatusChangedAsync(Employee employee, EmployeeStatus oldStatus, EmployeeStatus newStatus)
    {
        await _gameHub.Clients.All.SendAsync("EmployeeStatusChanged", employee, oldStatus, newStatus);
    }

    public async Task NotifyEmployeeCreatedAsync(Employee employee)
    {
        await _gameHub.Clients.All.SendAsync("EmployeeCreated", employee);
    }

    public async Task NotifyRefreshAllBoardsAsync()
    {
        await _gameHub.Clients.All.SendAsync("RefreshAllBoards");
    }
}
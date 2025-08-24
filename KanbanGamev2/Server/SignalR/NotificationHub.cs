using Microsoft.AspNetCore.SignalR;
using KanbanGamev2.Client.Services;

namespace KanbanGamev2.Server.SignalR;

public class NotificationHub : Hub
{
    public async Task SendGlobalNotification(string title, string message, string type)
    {
        await Clients.All.SendAsync("ReceiveGlobalNotification", title, message, type);
    }
} 
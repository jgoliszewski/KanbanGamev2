using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using KanbanGame.Shared;

namespace KanbanGamev2.Server.SignalR;

public class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string, UserInfo> _connectedUsers = new();
    private static readonly ConcurrentDictionary<string, bool> _readyUsers = new();

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var userInfo = new UserInfo
        {
            ConnectionId = connectionId,
            ConnectedAt = DateTime.UtcNow,
            UserName = $"User_{connectionId.Substring(0, 8)}"
        };

        _connectedUsers.TryAdd(connectionId, userInfo);
        
        await Clients.All.SendAsync("UserConnected", userInfo);
        await Clients.All.SendAsync("UpdateConnectedCount", _connectedUsers.Count);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        
        _connectedUsers.TryRemove(connectionId, out _);
        _readyUsers.TryRemove(connectionId, out _);
        
        await Clients.All.SendAsync("UserDisconnected", connectionId);
        await Clients.All.SendAsync("UpdateConnectedCount", _connectedUsers.Count);
        await Clients.All.SendAsync("UpdateReadyCount", _readyUsers.Count);
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SetReadyStatus(bool isReady)
    {
        var connectionId = Context.ConnectionId;
        
        if (isReady)
        {
            _readyUsers.TryAdd(connectionId, true);
        }
        else
        {
            _readyUsers.TryRemove(connectionId, out _);
        }
        
        await Clients.All.SendAsync("UpdateReadyCount", _readyUsers.Count);
        await Clients.All.SendAsync("UserReadyStatusChanged", connectionId, isReady);
        
        // Check if all connected users are ready
        if (_readyUsers.Count > 0 && _readyUsers.Count == _connectedUsers.Count)
        {
            await Clients.All.SendAsync("AllPlayersReady");
        }
    }

    public async Task GetCurrentStats()
    {
        await Clients.Caller.SendAsync("UpdateConnectedCount", _connectedUsers.Count);
        await Clients.Caller.SendAsync("UpdateReadyCount", _readyUsers.Count);
    }

    public async Task AdvanceToNextDay()
    {
        // Reset all users to not ready
        _readyUsers.Clear();
        
        // Notify all clients about the next day
        await Clients.All.SendAsync("NextDayStarted");
        await Clients.All.SendAsync("UpdateReadyCount", 0);
        
        // Notify all clients to reload their game state and refresh boards
        await Clients.All.SendAsync("ReloadGameState");
        await Clients.All.SendAsync("RefreshAllBoards");
    }

    public async Task NotifyBoardUpdate(string boardType, string columnId, object cardData)
    {
        await Clients.All.SendAsync("BoardUpdated", boardType, columnId, cardData);
    }

    public async Task NotifyEmployeeMove(Employee employee, BoardType boardType, string columnId, BoardType originalBoardType)
    {
        await Clients.All.SendAsync("EmployeeMoved", employee, boardType, columnId, originalBoardType);
    }

    public async Task NotifyEmployeeStatusChanged(Employee employee, EmployeeStatus oldStatus, EmployeeStatus newStatus)
    {
        await Clients.All.SendAsync("EmployeeStatusChanged", employee, oldStatus, newStatus);
    }

    public static int GetConnectedCount() => _connectedUsers.Count;
    public static int GetReadyCount() => _readyUsers.Count;
}

public class UserInfo
{
    public string ConnectionId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
} 
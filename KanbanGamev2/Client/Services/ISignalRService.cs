namespace KanbanGamev2.Client.Services;

public interface ISignalRService
{
    event Action<int>? ConnectedCountChanged;
    event Action<int>? ReadyCountChanged;
    event Action<UserInfo>? UserConnected;
    event Action<string>? UserDisconnected;
    event Action<string, bool>? UserReadyStatusChanged;
    event Action? AllPlayersReady;
    event Action? NextDayStarted;
    event Action? ReloadGameState;
    event Action<string, string, object>? BoardUpdated;
    event Action<string, string>? ShowLoader;
    event Action? HideLoader;
    
    bool IsConnected { get; }
    int ConnectedCount { get; }
    int ReadyCount { get; }
    
    Task ConnectAsync();
    Task DisconnectAsync();
    Task SetReadyStatusAsync(bool isReady);
    Task GetCurrentStatsAsync();
    Task AdvanceToNextDayAsync();
    Task NotifyBoardUpdateAsync(string boardType, string columnId, object cardData);
}

public class UserInfo
{
    public string ConnectionId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
} 
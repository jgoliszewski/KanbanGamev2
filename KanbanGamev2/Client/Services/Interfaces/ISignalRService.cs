using KanbanGame.Shared;

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
    event Action? RefreshAllBoards;
    event Action<string, string, object>? BoardUpdated;
    event Action<Employee, BoardType, string, BoardType>? EmployeeMoved;
    event Action<string, string>? ShowLoader;
    event Action? HideLoader;
    event Action<string, string, string>? GlobalNotificationReceived;
    event Action<string, string, string>? BoardUpdateReceived;
    event Action<string, string, string>? EmployeeMoveReceived;
    event Action<Employee, EmployeeStatus, EmployeeStatus>? EmployeeStatusChanged;
    event Action<bool>? SummaryBoardVisibilityChangedFromServer;

    bool IsConnected { get; }
    int ConnectedCount { get; }
    int ReadyCount { get; }

    Task ConnectAsync();
    Task DisconnectAsync();
    Task SetReadyStatusAsync(bool isReady);
    Task GetCurrentStatsAsync();
    Task AdvanceToNextDayAsync();
    Task NotifyBoardUpdateAsync(string boardType, string columnId, object cardData);
    Task NotifyEmployeeMoveAsync(Employee employee, BoardType boardType, string columnId, BoardType originalBoardType);
}

public class UserInfo
{
    public string ConnectionId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
}
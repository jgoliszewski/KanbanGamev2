using Microsoft.AspNetCore.SignalR.Client;

namespace KanbanGamev2.Client.Services;

public class SignalRService : ISignalRService, IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private bool _isReady = false;
    private readonly string _baseUrl;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    public int ConnectedCount { get; private set; }
    public int ReadyCount { get; private set; }

    public event Action<int>? ConnectedCountChanged;
    public event Action<int>? ReadyCountChanged;
    public event Action<UserInfo>? UserConnected;
    public event Action<string>? UserDisconnected;
    public event Action<string, bool>? UserReadyStatusChanged;
    public event Action? AllPlayersReady;
    public event Action? NextDayStarted;
    public event Action? ReloadGameState;
    public event Action<string, string, object>? BoardUpdated;
    public event Action<string, string>? ShowLoader;
    public event Action? HideLoader;
    public event Action? RefreshAllBoards;

    public SignalRService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task ConnectAsync()
    {
        if (_hubConnection != null)
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_baseUrl}gamehub")
            .WithAutomaticReconnect()
            .Build();

        // Set up event handlers
        _hubConnection.On<int>("UpdateConnectedCount", count =>
        {
            ConnectedCount = count;
            ConnectedCountChanged?.Invoke(count);
        });

        _hubConnection.On<int>("UpdateReadyCount", count =>
        {
            ReadyCount = count;
            ReadyCountChanged?.Invoke(count);
        });

        _hubConnection.On<UserInfo>("UserConnected", userInfo =>
        {
            UserConnected?.Invoke(userInfo);
        });

        _hubConnection.On<string>("UserDisconnected", connectionId =>
        {
            UserDisconnected?.Invoke(connectionId);
        });

        _hubConnection.On<string, bool>("UserReadyStatusChanged", (connectionId, isReady) =>
        {
            UserReadyStatusChanged?.Invoke(connectionId, isReady);
        });

        _hubConnection.On("AllPlayersReady", () =>
        {
            AllPlayersReady?.Invoke();
        });

        _hubConnection.On("NextDayStarted", () =>
        {
            ShowLoader?.Invoke("Day Advanced", "The game day has been advanced. Updating your view...");
            NextDayStarted?.Invoke();
            // Hide loader after a short delay
            Task.Delay(2000).ContinueWith(_ => HideLoader?.Invoke());
        });

        _hubConnection.On("ReloadGameState", () =>
        {
            ReloadGameState?.Invoke();
        });

        _hubConnection.On("RefreshAllBoards", () =>
        {
            RefreshAllBoards?.Invoke();
        });

        _hubConnection.On<string, string, object>("BoardUpdated", (boardType, columnId, cardData) =>
        {
            BoardUpdated?.Invoke(boardType, columnId, cardData);
        });

        // Handle connection state changes
        _hubConnection.Closed += async (error) =>
        {
            Console.WriteLine($"SignalR connection closed: {error?.Message}");
            await Task.CompletedTask;
        };

        _hubConnection.Reconnecting += (error) =>
        {
            Console.WriteLine($"SignalR reconnecting: {error?.Message}");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += (connectionId) =>
        {
            Console.WriteLine($"SignalR reconnected: {connectionId}");
            return Task.CompletedTask;
        };

        try
        {
            ShowLoader?.Invoke("Connecting to Game Server", "Establishing real-time connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine($"SignalR connected successfully to {_baseUrl}gamehub");
            await GetCurrentStatsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR connection failed: {ex.Message}");
            Console.WriteLine($"Attempted to connect to: {_baseUrl}gamehub");
        }
        finally
        {
            HideLoader?.Invoke();
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async Task SetReadyStatusAsync(bool isReady)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            _isReady = isReady;
            await _hubConnection.InvokeAsync("SetReadyStatus", isReady);
        }
    }

    public async Task GetCurrentStatsAsync()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("GetCurrentStats");
        }
    }

    public async Task AdvanceToNextDayAsync()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("AdvanceToNextDay");
        }
    }

    public async Task NotifyBoardUpdateAsync(string boardType, string columnId, object cardData)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("NotifyBoardUpdate", boardType, columnId, cardData);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
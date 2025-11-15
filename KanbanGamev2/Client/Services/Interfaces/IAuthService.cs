namespace KanbanGamev2.Client.Services;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    event Action<bool>? AuthenticationStateChanged;
    Task<bool> LoginAsync(string password);
    void Logout();
    Task CheckAuthenticationAsync();
}


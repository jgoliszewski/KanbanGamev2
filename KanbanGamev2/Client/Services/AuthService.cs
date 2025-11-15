using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace KanbanGamev2.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private const string AuthKey = "gamemaster_authenticated";
    private const string SessionKey = "gamemaster_session";

    public bool IsAuthenticated { get; private set; }
    public event Action<bool>? AuthenticationStateChanged;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> LoginAsync(string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Password = password });
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result?.IsAuthenticated == true)
                {
                    IsAuthenticated = true;
                    // Store in sessionStorage
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", AuthKey, "true");
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", SessionKey, DateTime.UtcNow.Ticks.ToString());
                    AuthenticationStateChanged?.Invoke(true);
                    return true;
                }
            }
            
            IsAuthenticated = false;
            AuthenticationStateChanged?.Invoke(false);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            IsAuthenticated = false;
            AuthenticationStateChanged?.Invoke(false);
            return false;
        }
    }

    public void Logout()
    {
        IsAuthenticated = false;
        _ = _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", AuthKey);
        _ = _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);
        AuthenticationStateChanged?.Invoke(false);
    }

    public async Task CheckAuthenticationAsync()
    {
        try
        {
            var authValue = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", AuthKey);
            var sessionValue = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", SessionKey);
            
            if (!string.IsNullOrEmpty(authValue) && authValue == "true" && !string.IsNullOrEmpty(sessionValue))
            {
                // Verify session is still valid (24 hours)
                if (long.TryParse(sessionValue, out var sessionTicks))
                {
                    var sessionTime = new DateTime(sessionTicks);
                    if (DateTime.UtcNow - sessionTime < TimeSpan.FromHours(24))
                    {
                        IsAuthenticated = true;
                        AuthenticationStateChanged?.Invoke(true);
                        return;
                    }
                }
            }
            
            // If we get here, authentication is invalid
            IsAuthenticated = false;
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", AuthKey);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);
            AuthenticationStateChanged?.Invoke(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Check authentication error: {ex.Message}");
            IsAuthenticated = false;
            AuthenticationStateChanged?.Invoke(false);
        }
    }

    private class AuthResponse
    {
        public bool IsAuthenticated { get; set; }
    }
}


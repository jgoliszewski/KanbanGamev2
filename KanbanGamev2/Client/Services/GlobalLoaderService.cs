namespace KanbanGamev2.Client.Services;

public interface IGlobalLoaderService
{
    bool IsVisible { get; }
    string Title { get; }
    string Message { get; }
    
    event Action<bool, string, string>? LoaderStateChanged;
    
    void Show(string title = "Loading...", string message = "Please wait...");
    void Hide();
}

public class GlobalLoaderService : IGlobalLoaderService
{
    private bool _isVisible = false;
    private string _title = "Loading...";
    private string _message = "Please wait...";

    public bool IsVisible => _isVisible;
    public string Title => _title;
    public string Message => _message;

    public event Action<bool, string, string>? LoaderStateChanged;

    public void Show(string title = "Loading...", string message = "Please wait...")
    {
        _isVisible = true;
        _title = title;
        _message = message;
        LoaderStateChanged?.Invoke(_isVisible, _title, _message);
    }

    public void Hide()
    {
        _isVisible = false;
        LoaderStateChanged?.Invoke(_isVisible, _title, _message);
    }
} 
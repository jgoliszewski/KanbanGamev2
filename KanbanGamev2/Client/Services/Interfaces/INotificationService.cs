namespace KanbanGamev2.Client.Services;

public interface INotificationService
{
    event Action<NotificationMessage>? NotificationReceived;
    Task ShowNotificationAsync(string title, string message, NotificationType type = NotificationType.Info);
    Task ShowGlobalNotificationAsync(string title, string message, NotificationType type = NotificationType.Info);
}

public class NotificationMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsGlobal { get; set; }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
} 
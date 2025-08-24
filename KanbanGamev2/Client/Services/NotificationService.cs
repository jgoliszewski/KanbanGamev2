using KanbanGamev2.Client.Services;

namespace KanbanGamev2.Client.Services;

public class NotificationService : INotificationService
{
    public event Action<NotificationMessage>? NotificationReceived;

    public Task ShowNotificationAsync(string title, string message, NotificationType type = NotificationType.Info)
    {
        var notification = new NotificationMessage
        {
            Title = title,
            Message = message,
            Type = type,
            IsGlobal = false
        };

        NotificationReceived?.Invoke(notification);
        return Task.CompletedTask;
    }

    public Task ShowGlobalNotificationAsync(string title, string message, NotificationType type = NotificationType.Info)
    {
        var notification = new NotificationMessage
        {
            Title = title,
            Message = message,
            Type = type,
            IsGlobal = true
        };

        NotificationReceived?.Invoke(notification);
        return Task.CompletedTask;
    }
} 
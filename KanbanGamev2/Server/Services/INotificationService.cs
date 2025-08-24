using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface INotificationService
{
    Task SendGlobalNotificationAsync(string title, string message, string type);
    Task RefreshAllBoardsAsync();
    Task NotifyEmployeeStatusChangedAsync(Employee employee, EmployeeStatus oldStatus, EmployeeStatus newStatus);
} 
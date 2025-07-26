using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface ITaskService
{
    Task<List<KanbanTask>> GetTasksAsync();
    Task<KanbanTask?> GetTaskAsync(Guid id);
    Task<KanbanTask> CreateTaskAsync(KanbanTask task);
    Task<KanbanTask> UpdateTaskAsync(KanbanTask task);
    Task<bool> DeleteTaskAsync(Guid id);
    Task<List<KanbanTask>> GetTasksByColumnAsync(string columnId);
} 
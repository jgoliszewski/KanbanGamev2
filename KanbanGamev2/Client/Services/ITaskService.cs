using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface ITaskService
{
    List<KanbanTask> Tasks { get; set; }
    Task GetTasks();
    Task<KanbanTask?> GetTask(Guid id);
    Task<KanbanTask> CreateTask(KanbanTask task);
    Task<KanbanTask> UpdateTask(KanbanTask task);
    Task<bool> DeleteTask(Guid id);
    Task<List<KanbanTask>> GetTasksByColumn(string columnId);
} 
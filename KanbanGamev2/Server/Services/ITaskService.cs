using KanbanGame.Shared;

namespace KanbanGamev2.Server.Services;

public interface ITaskService
{
    List<KanbanTask> GetTasks();
    KanbanTask? GetTask(Guid id);
    KanbanTask CreateTask(KanbanTask task);
    KanbanTask UpdateTask(KanbanTask task);
    bool DeleteTask(Guid id);
    List<KanbanTask> GetTasksByColumn(string columnId);
    void ResetData();
} 
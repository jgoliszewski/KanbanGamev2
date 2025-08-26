using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface ITaskService
{
    List<KanbanTask> Tasks { get; set; }
    Task GetTasks();
    Task<KanbanTask> UpdateTask(KanbanTask task);
    Task<bool> DeleteTask(Guid id);
    Task<bool> AreAllTasksCompleted(List<Guid> taskIds);
    Task DeleteTasks(List<Guid> taskIds);
}
namespace KanbanGamev2.Client.Services;

public interface IGameRestartService
{
    Task<bool> RestartGameAsync();
} 
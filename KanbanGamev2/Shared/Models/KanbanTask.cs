using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class KanbanTask : Card
{
    public Priority Priority { get; set; } = Priority.Medium;
    public Status Status { get; set; } = Status.ToDo;
    public Guid? AssignedToEmployeeId { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; } = 1; // Story points for the task
    public double LaborIntensity { get; set; } = 1.0; // Original labor intensity (0-1 scale)
    public double LaborLeft { get; set; } = 1.0; // Remaining work (0-1 scale)
    
    // Computed property to check if task is assigned
    public bool IsAssigned => AssignedToEmployeeId.HasValue;
    
    // Computed property to check if task is completed
    public bool IsCompleted => LaborLeft <= 0;
    
    // Computed property to check if task is in done column
    public bool IsInDoneColumn => ColumnId?.Contains("done") == true;
}
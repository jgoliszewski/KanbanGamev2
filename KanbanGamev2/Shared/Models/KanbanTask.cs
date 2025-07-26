using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class KanbanTask : Card
{
    public Priority Priority { get; set; } = Priority.Medium;
    public Status Status { get; set; } = Status.ToDo;
    public Guid? AssignedToEmployeeId { get; set; }
    public DateTime? DueDate { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    
    // Computed property to check if task is assigned
    public bool IsAssigned => AssignedToEmployeeId.HasValue;
}
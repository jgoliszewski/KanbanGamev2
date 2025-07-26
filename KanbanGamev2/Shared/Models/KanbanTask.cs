using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class KanbanTask : Card
{
    public Priority Priority { get; set; } = Priority.Medium;
    public Status Status { get; set; } = Status.ToDo;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
}
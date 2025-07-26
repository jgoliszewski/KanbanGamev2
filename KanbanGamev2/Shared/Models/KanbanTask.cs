namespace KanbanGame.Shared;

public class KanbanTask : Card
{
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "To Do";
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
}
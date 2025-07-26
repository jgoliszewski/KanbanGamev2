using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Feature : Card
{
    public Priority Priority { get; set; } = Priority.Medium;
    public Status Status { get; set; } = Status.New;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
}
using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Feature : Card
{
    public Priority Priority { get; set; } = Priority.Medium;
    public Status Status { get; set; } = Status.New;
    public Guid? AssignedToEmployeeId { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
    
    // Computed property to check if feature is assigned
    public bool IsAssigned => AssignedToEmployeeId.HasValue;
}
using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Feature : Card
{
    public Priority Priority { get; set; } = Priority.Medium;
    public Status Status { get; set; } = Status.New;
    public Guid? AssignedToEmployeeId { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
    public double LaborIntensity { get; set; } = 1.0; // Original labor intensity (0-1 scale)
    public double LaborLeft { get; set; } = 1.0; // Remaining work (0-1 scale)
    
    // Computed property to check if feature is assigned
    public bool IsAssigned => AssignedToEmployeeId.HasValue;
    
    // Computed property to check if feature is completed
    public bool IsCompleted => LaborLeft <= 0;
    
    // Computed property to check if feature is in ready-dev column
    public bool IsInReadyDevColumn => ColumnId?.Contains("ready-dev") == true;
}
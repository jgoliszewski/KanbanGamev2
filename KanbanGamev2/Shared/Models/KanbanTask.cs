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
    
    // Dependency properties
    public Guid? DependsOnTaskId { get; set; } // ID of the task this task depends on
    public Guid FeatureId { get; set; } // ID of the feature this task belongs to
    public BoardType BoardType { get; set; } // Board type (Backend/Frontend) for dependency validation
    
    // Computed property to check if task is assigned
    public bool IsAssigned => AssignedToEmployeeId.HasValue;
    
    // Computed property to check if task is completed
    public bool IsCompleted => LaborLeft <= 0;
    
    // Computed property to check if task is in done column
    public bool IsInDoneColumn => ColumnId?.Contains("done") == true;
    
    // Computed property to check if task has dependencies
    public bool HasDependency => DependsOnTaskId.HasValue;
    
    // Computed property to check if task can be moved (no blocking dependencies)
    public bool CanBeMoved => !HasDependency || IsDependencySatisfied;
    
    // Computed property to check if dependency is satisfied
    public bool IsDependencySatisfied => !HasDependency || (DependsOnTaskId.HasValue && IsDependencyCompleted);
    
    // Helper method to check if dependency is completed (to be implemented in service layer)
    public bool IsDependencyCompleted { get; set; } = false;
}
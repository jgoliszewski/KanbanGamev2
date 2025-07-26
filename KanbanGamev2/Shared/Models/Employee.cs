using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Employee : Card
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Developer;
    public Department Department { get; set; } = Department.Engineering;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    
    // Work assignment properties
    public Guid? AssignedTaskId { get; set; }
    public Guid? AssignedFeatureId { get; set; }
    
    // Computed property to check if employee is working on something
    public bool IsWorking => AssignedTaskId.HasValue || AssignedFeatureId.HasValue;
}

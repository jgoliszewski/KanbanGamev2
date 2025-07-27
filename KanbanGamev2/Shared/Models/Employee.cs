using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Employee : Card
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Developer;
    public Department Department { get; set; } = Department.Engineering;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public Seniority Seniority { get; set; } = Seniority.Junior;
    
    // Work assignment properties
    public Guid? AssignedTaskId { get; set; }
    public Guid? AssignedFeatureId { get; set; }
    
    // Computed property to check if employee is working on something
    public bool IsWorking => AssignedTaskId.HasValue || AssignedFeatureId.HasValue;
    
    // Efficiency based on seniority (0-1 scale)
    public double Efficiency => Seniority switch
    {
        Seniority.Junior => 0.3,
        Seniority.Mid => 0.6,
        Seniority.Senior => 0.9,
        _ => 0.3
    };
}

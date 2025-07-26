using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Employee : Card
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Developer;
    public Department Department { get; set; } = Department.Engineering;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}

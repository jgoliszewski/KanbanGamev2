namespace KanbanGame.Shared;

public class Employee : Card
{
    public string Name { get; set; } = string.Empty;
    public List<Role> LearnedRoles { get; set; } = new List<Role>();
    public List<Role> LearnableRoles { get; set; } = new List<Role>();
    public Department Department { get; set; } = Department.Engineering;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public Seniority Seniority { get; set; } = Seniority.Junior;
    public BoardType BoardType { get; set; } = BoardType.Analysis;

    // Work assignment properties
    public Guid? AssignedTaskId { get; set; }
    public Guid? AssignedFeatureId { get; set; }

    // Employee status properties
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public DateTime? VacationStartDate { get; set; }
    public DateTime? VacationEndDate { get; set; }

    // Computed property to check if employee is working on something
    public bool IsWorking => AssignedTaskId.HasValue || AssignedFeatureId.HasValue;

    // Efficiency based on seniority (0-1 scale)
    public double Efficiency => Seniority switch
    {
        Seniority.Junior => 1,
        Seniority.Mid => 1,
        Seniority.Senior => 1,
        _ => 0.3
    };

    // Check if employee can work in a specific column based on their learned roles
    public bool CanWorkInColumn(string columnId)
    {
        return columnId switch
        {
            // Analysis columns - only analysts can work here
            "analysis1" or "analysis2" => LearnedRoles.Contains(Role.Analyst) || LearnedRoles.Contains(Role.HighLevelAnalyst),

            // Backend analysis - only analysts can work here
            "backend-analysis" => LearnedRoles.Contains(Role.Analyst) || LearnedRoles.Contains(Role.HighLevelAnalyst),

            // Frontend analysis - only analysts can work here
            "frontend-analysis" => LearnedRoles.Contains(Role.Analyst) || LearnedRoles.Contains(Role.HighLevelAnalyst),

            // Development columns - only developers can work here
            "backend-dev-doing" or "frontend-dev-doing" => LearnedRoles.Contains(Role.Developer),

            // Testing columns - only testers can work here
            "backend-test-doing" or "frontend-test-doing" => LearnedRoles.Contains(Role.Tester),

            // Default - allow all roles
            _ => true
        };
    }

    // Get all roles the employee currently has
    public List<Role> GetAllRoles()
    {
        return LearnedRoles.ToList();
    }

    // Check if employee can learn a specific role
    public bool CanLearnRole(Role role)
    {
        return LearnableRoles.Contains(role);
    }

    // Check if a role is blocked for this employee
    public bool IsRoleBlocked(Role role)
    {
        return !LearnedRoles.Contains(role) && !LearnableRoles.Contains(role);
    }

    // Check if employee has a specific role
    public bool HasRole(Role role)
    {
        return LearnedRoles.Contains(role);
    }
}

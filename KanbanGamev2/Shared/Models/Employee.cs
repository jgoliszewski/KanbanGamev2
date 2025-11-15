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
    public DateTime? OnboardingEndDate { get; set; }

    // Learning properties
    public int LearningDays { get; set; } = 0;
    public Role? LearningRole { get; set; } = null;
    public const int DaysRequiredToLearnRole = 8;

    // Team changing properties
    public int ChangingTeamsDays { get; set; } = 0;
    public BoardType? PreviousBoardType { get; set; } = null;
    public const int DaysRequiredToChangeTeams = 3;

    // Computed property to check if employee is working on something
    public bool IsWorking => AssignedTaskId.HasValue || AssignedFeatureId.HasValue;

    // Computed property to check if employee is learning
    public bool IsLearning => Status == EmployeeStatus.IsLearning || Status == EmployeeStatus.IsLearningInOtherTeam;

    // Computed property to check if employee is learning in other team
    public bool IsLearningInOtherTeam => Status == EmployeeStatus.IsLearningInOtherTeam;

    // Computed property to check if employee is changing teams
    public bool IsChangingTeams => Status == EmployeeStatus.ChangingTeams;

    // Computed property to check if employee is onboarding
    public bool IsOnboarding => Status == EmployeeStatus.Onboarding;

    // Efficiency based on seniority (0-1 scale)
    public double Efficiency => Seniority switch
    {
        Seniority.Junior => 1,
        Seniority.Mid => 1,
        Seniority.Senior => 1,
        _ => 0.3
    };

    // Get the role required for a specific column
    public Role? GetRoleRequiredForColumn(string columnId)
    {
        return columnId switch
        {
            // High-level analysis columns - require HighLevelAnalyst
            "analysis1" or "analysis2" => Role.HighLevelAnalyst,

            // Backend and frontend analysis - require Analyst
            "backend-analysis" or "frontend-analysis" => Role.Analyst,

            // Development columns - require Developer
            "backend-dev-doing" or "frontend-dev-doing" => Role.Developer,

            // Testing columns - require Tester
            "backend-test-doing" or "frontend-test-doing" => Role.Tester,

            // Default - no specific role required
            _ => null
        };
    }

    // Check if employee can be placed in a column (either has the role or can learn it)
    public bool CanBePlacedInColumn(string columnId)
    {
        var requiredRole = GetRoleRequiredForColumn(columnId);
        
        // If no role required, allow placement
        if (requiredRole == null)
            return true;

        // If employee has the role, they can be placed
        if (LearnedRoles.Contains(requiredRole.Value))
            return true;

        // If employee can learn the role, they can be placed (to learn it)
        if (CanLearnRole(requiredRole.Value))
            return true;

        return false;
    }

    // Check if employee should be learning in a column (can learn but doesn't have the role)
    public bool ShouldBeLearningInColumn(string columnId)
    {
        var requiredRole = GetRoleRequiredForColumn(columnId);
        
        // If no role required, no learning needed
        if (requiredRole == null)
            return false;

        // If employee already has the role, no learning needed
        if (LearnedRoles.Contains(requiredRole.Value))
            return false;

        // If employee can learn the role but doesn't have it, they should be learning
        return CanLearnRole(requiredRole.Value);
    }

    // Check if employee can work in a specific column based on their learned roles
    public bool CanWorkInColumn(string columnId)
    {
        // If employee is onboarding, they cannot work
        if (IsOnboarding)
            return false;

        // If employee is learning, they cannot work
        if (IsLearning)
            return false;

        // If employee is changing teams, they cannot work
        if (IsChangingTeams)
            return false;

        return columnId switch
        {
            // High-level analysis columns - only HighLevelAnalyst can work here
            "analysis1" => LearnedRoles.Contains(Role.HighLevelAnalyst),
            "analysis2" => LearnedRoles.Contains(Role.HighLevelAnalyst),

            // Backend analysis - only regular Analyst can work here (not HighLevelAnalyst)
            "backend-analysis" => LearnedRoles.Contains(Role.Analyst) && !LearnedRoles.Contains(Role.HighLevelAnalyst),

            // Frontend analysis - only regular Analyst can work here (not HighLevelAnalyst)
            "frontend-analysis" => LearnedRoles.Contains(Role.Analyst) && !LearnedRoles.Contains(Role.HighLevelAnalyst),

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

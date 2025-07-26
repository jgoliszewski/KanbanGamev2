namespace KanbanGame.Shared;

public class Feature : Card
{
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "New";
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
}
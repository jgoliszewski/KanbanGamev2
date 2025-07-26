namespace KanbanGame.Shared;

public class Board
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Column> Columns { get; set; } = new List<Column>();
}

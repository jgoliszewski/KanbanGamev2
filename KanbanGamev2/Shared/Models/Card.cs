namespace KanbanGame.Shared;

public class Card
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string ColumnId { get; set; } = string.Empty;
    public int Order { get; set; }
}
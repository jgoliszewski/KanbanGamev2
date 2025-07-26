using KanbanGame.Shared;

namespace KanbanGame.Shared;

public class Column
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BoardId { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<Card> Cards { get; set; } = new List<Card>();
}

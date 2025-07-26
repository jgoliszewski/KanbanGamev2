using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public class DragDropService : IDragDropService
{
    public Card? DraggedCard { get; set; }
    public string? SourceColumnId { get; set; }
    public string? TargetColumnId { get; set; }
    
    public bool IsDragging => DraggedCard != null;

    public void StartDrag(Card card, string columnId)
    {
        DraggedCard = card;
        SourceColumnId = columnId;
        TargetColumnId = null;
    }

    public void SetDropTarget(string columnId)
    {
        TargetColumnId = columnId;
    }

    public void ClearDrag()
    {
        DraggedCard = null;
        SourceColumnId = null;
        TargetColumnId = null;
    }
} 
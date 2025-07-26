using KanbanGame.Shared;

namespace KanbanGamev2.Client.Services;

public interface IDragDropService
{
    Card? DraggedCard { get; set; }
    string? SourceColumnId { get; set; }
    string? TargetColumnId { get; set; }
    
    void StartDrag(Card card, string columnId);
    void SetDropTarget(string columnId);
    void ClearDrag();
    bool IsDragging { get; }
    
    // Validation methods
    bool IsValidMove(BoardType boardType, Card card, string fromColumn, string toColumn);
    bool IsReadOnlyBoard(BoardType boardType);
    
    // Work assignment methods
    bool CanAssignWork(Card workCard, Employee employee);
    bool CanMoveWorkForward(BoardType boardType, Card workCard, Employee employee);
    void AssignWorkToEmployee(Card workCard, Employee employee);
    void UnassignWorkFromEmployee(Employee employee);
    void UnassignWorkFromCard(Card card);
    void UnassignWorkWhenMoved(Card card);
} 
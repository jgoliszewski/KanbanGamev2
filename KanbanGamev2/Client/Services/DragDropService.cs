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

    public bool IsReadOnlyBoard(BoardType boardType)
    {
        return boardType == BoardType.Summary;
    }

    public bool IsValidMove(BoardType boardType, Card card, string fromColumn, string toColumn)
    {
        // Summary board is read-only
        if (IsReadOnlyBoard(boardType))
            return false;

        // Same column is always valid (no-op)
        if (fromColumn == toColumn)
            return true;

        switch (boardType)
        {
            case BoardType.Analysis:
                return IsValidAnalysisMove(card, fromColumn, toColumn);
            case BoardType.Backend:
            case BoardType.Frontend:
                return IsValidDevBoardMove(card, fromColumn, toColumn);
            default:
                return false;
        }
    }

    private bool IsValidAnalysisMove(Card card, string fromColumn, string toColumn)
    {
        if (card is Employee)
        {
            // Employees can only move between "under analysis 1" and "under analysis 2"
            var allowedEmployeeColumns = new[] { "analysis1", "analysis2" };
            return allowedEmployeeColumns.Contains(fromColumn) && allowedEmployeeColumns.Contains(toColumn);
        }
        else if (card is Feature)
        {
            // Features can move in pairs:
            // - "backlog" <-> "analysis1" (under analysis 1)
            // - "waiting" <-> "analysis2" (under analysis 2)
            var backlogAnalysis1Pair = new[] { "backlog", "analysis1" };
            var waitingAnalysis2Pair = new[] { "waiting", "analysis2" };

            return (backlogAnalysis1Pair.Contains(fromColumn) && backlogAnalysis1Pair.Contains(toColumn)) ||
                   (waitingAnalysis2Pair.Contains(fromColumn) && waitingAnalysis2Pair.Contains(toColumn));
        }

        return false;
    }

    private bool IsValidDevBoardMove(Card card, string fromColumn, string toColumn)
    {
        if (card is Employee)
        {
            // Employees can only move between Analysis, Development Doing, and Testing Doing
            var allowedEmployeeColumns = new[] { "backend-analysis", "frontend-analysis", "backend-dev-doing", "frontend-dev-doing", "backend-test-doing", "frontend-test-doing" };
            return allowedEmployeeColumns.Contains(fromColumn) && allowedEmployeeColumns.Contains(toColumn);
        }
        else if (card is KanbanTask)
        {
            // Tasks can move in column pairs:
            // - "backend-backlog" <-> "backend-analysis" (or frontend equivalents)
            // - "backend-dev-waiting" <-> "backend-dev-doing" (or frontend equivalents)
            // - "backend-test-waiting" <-> "backend-test-doing" (or frontend equivalents)

            var backlogAnalysisPair = new[] { "backend-backlog", "frontend-backlog", "backend-analysis", "frontend-analysis" };
            var devWaitingDoingPair = new[] { "backend-dev-waiting", "frontend-dev-waiting", "backend-dev-doing", "frontend-dev-doing" };
            var testWaitingDoingPair = new[] { "backend-test-waiting", "frontend-test-waiting", "backend-test-doing", "frontend-test-doing" };

            return (backlogAnalysisPair.Contains(fromColumn) && backlogAnalysisPair.Contains(toColumn)) ||
                   (devWaitingDoingPair.Contains(fromColumn) && devWaitingDoingPair.Contains(toColumn)) ||
                   (testWaitingDoingPair.Contains(fromColumn) && testWaitingDoingPair.Contains(toColumn));
        }

        return false;
    }
} 
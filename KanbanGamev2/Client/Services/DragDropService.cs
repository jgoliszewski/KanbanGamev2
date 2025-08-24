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

        // Working employees cannot be moved between columns
        if (card is Employee employee && employee.IsWorking)
            return false;

        // For work items (tasks/features), only backward moves are allowed via column drops
        // Forward moves must be done by dropping on employees
        if (card is KanbanTask || card is Feature)
        {
            return IsValidBackwardMove(boardType, card, fromColumn, toColumn);
        }

        // For employees, use the existing validation
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

    private bool IsValidBackwardMove(BoardType boardType, Card card, string fromColumn, string toColumn)
    {
        switch (boardType)
        {
            case BoardType.Analysis:
                // Analysis board: only backward moves allowed
                // "analysis1" -> "backlog"
                // "analysis2" -> "waiting"
                return (fromColumn == "analysis1" && toColumn == "backlog") ||
                       (fromColumn == "analysis2" && toColumn == "waiting");
            case BoardType.Backend:
            case BoardType.Frontend:
                // Development boards: only backward moves allowed
                // "backend-analysis"/"frontend-analysis" -> "backend-backlog"/"frontend-backlog"
                // "backend-dev-doing"/"frontend-dev-doing" -> "backend-dev-waiting"/"frontend-dev-waiting"
                // "backend-test-doing"/"frontend-test-doing" -> "backend-test-waiting"/"frontend-test-waiting"
                return (fromColumn == "backend-analysis" && toColumn == "backend-backlog") ||
                       (fromColumn == "frontend-analysis" && toColumn == "frontend-backlog") ||
                       (fromColumn == "backend-dev-doing" && toColumn == "backend-dev-waiting") ||
                       (fromColumn == "frontend-dev-doing" && toColumn == "frontend-dev-waiting") ||
                       (fromColumn == "backend-test-doing" && toColumn == "backend-test-waiting") ||
                       (fromColumn == "frontend-test-doing" && toColumn == "frontend-test-waiting");
            default:
                return false;
        }
    }

    public bool CanAssignWork(Card workCard, Employee employee)
    {
        // Check if the work card is a task or feature
        if (workCard is not KanbanTask && workCard is not Feature)
            return false;
            
        // Check if employee is already working on something
        if (employee.IsWorking)
            return false;
            
        // Check if the work is already assigned to someone
        if (workCard is KanbanTask task && task.IsAssigned)
            return false;
        if (workCard is Feature feature && feature.IsAssigned)
            return false;
            
        // Check if employee can work in the current column of the work item
        if (!employee.CanWorkInColumn(workCard.ColumnId))
            return false;
            
        return true;
    }

    public bool CanMoveWorkForward(BoardType boardType, Card workCard, Employee employee)
    {
        if (!CanAssignWork(workCard, employee))
            return false;

        // Check if employee can work in the target column
        if (!employee.CanWorkInColumn(employee.ColumnId))
            return false;

        // Check if this would be a valid forward move
        if (workCard is KanbanTask || workCard is Feature)
        {
            return IsValidForwardMove(boardType, workCard, employee.ColumnId);
        }

        return false;
    }

    private bool IsValidForwardMove(BoardType boardType, Card workCard, string targetColumn)
    {
        var currentColumn = workCard.ColumnId;

        switch (boardType)
        {
            case BoardType.Analysis:
                // Analysis board forward moves:
                // "backlog" -> "analysis1" (via employee in analysis1)
                // "waiting" -> "analysis2" (via employee in analysis2)
                return (currentColumn == "backlog" && targetColumn == "analysis1") ||
                       (currentColumn == "waiting" && targetColumn == "analysis2");
            case BoardType.Backend:
            case BoardType.Frontend:
                // Development board forward moves:
                // "backend-backlog"/"frontend-backlog" -> "backend-analysis"/"frontend-analysis"
                // "backend-dev-waiting"/"frontend-dev-waiting" -> "backend-dev-doing"/"frontend-dev-doing"
                // "backend-test-waiting"/"frontend-test-waiting" -> "backend-test-doing"/"frontend-test-doing"
                return (currentColumn == "backend-backlog" && targetColumn == "backend-analysis") ||
                       (currentColumn == "frontend-backlog" && targetColumn == "frontend-analysis") ||
                       (currentColumn == "backend-dev-waiting" && targetColumn == "backend-dev-doing") ||
                       (currentColumn == "frontend-dev-waiting" && targetColumn == "frontend-dev-doing") ||
                       (currentColumn == "backend-test-waiting" && targetColumn == "backend-test-doing") ||
                       (currentColumn == "frontend-test-waiting" && targetColumn == "frontend-test-doing");
            default:
                return false;
        }
    }

    public void AssignWorkToEmployee(Card workCard, Employee employee)
    {
        if (!CanAssignWork(workCard, employee))
            return;
            
        if (workCard is KanbanTask task)
        {
            task.AssignedToEmployeeId = employee.Id;
            employee.AssignedTaskId = task.Id;
        }
        else if (workCard is Feature feature)
        {
            feature.AssignedToEmployeeId = employee.Id;
            employee.AssignedFeatureId = feature.Id;
        }
    }

    public void UnassignWorkFromEmployee(Employee employee)
    {
        if (employee.AssignedTaskId.HasValue)
        {
            // Find and unassign the task
            // This will be handled by the service layer
            employee.AssignedTaskId = null;
        }
        
        if (employee.AssignedFeatureId.HasValue)
        {
            // Find and unassign the feature
            // This will be handled by the service layer
            employee.AssignedFeatureId = null;
        }
    }

    public void UnassignWorkFromCard(Card card)
    {
        if (card is KanbanTask task)
        {
            task.AssignedToEmployeeId = null;
        }
        else if (card is Feature feature)
        {
            feature.AssignedToEmployeeId = null;
        }
    }

    public void UnassignWorkWhenMoved(Card card)
    {
        if (card is Employee employee)
        {
            // If employee is moved, unassign their current work
            UnassignWorkFromEmployee(employee);
        }
        else if (card is KanbanTask || card is Feature)
        {
            // If work is moved, unassign it from the employee
            UnassignWorkFromCard(card);
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
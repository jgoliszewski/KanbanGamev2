namespace KanbanGame.Shared;

public class MoneyTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public TransactionType Type { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
} 
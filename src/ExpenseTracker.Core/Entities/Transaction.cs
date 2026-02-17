namespace ExpenseTracker.Core.Entities;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public bool IsIncome { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Category Category { get; set; } = null!;
}

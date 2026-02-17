namespace ExpenseTracker.Core.Entities;

public enum RecurrenceFrequency
{
    Weekly = 0,
    Monthly = 1
}

public class RecurringTransaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public bool IsIncome { get; set; }
    public string? Note { get; set; }
    public int CategoryId { get; set; }
    public RecurrenceFrequency Frequency { get; set; }
    public DateTime NextDueDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Category Category { get; set; } = null!;
}

namespace ExpenseTracker.Core.Entities;

public class Budget
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal AmountLimit { get; set; }

    public Category Category { get; set; } = null!;
}

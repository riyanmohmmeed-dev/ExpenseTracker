namespace ExpenseTracker.Core.DTOs;

public class MonthlySummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
    public List<CategorySummaryDto> ByCategory { get; set; } = new();
    public List<BudgetProgressDto> BudgetProgress { get; set; } = new();
}

public class CategorySummaryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public bool IsIncome { get; set; }
}

public class BudgetProgressDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Spent { get; set; }
    public decimal Limit { get; set; }
    public decimal Percentage => Limit > 0 ? Math.Round(100 * Spent / Limit, 1) : 0;
    public bool IsOverBudget => Spent > Limit;
}

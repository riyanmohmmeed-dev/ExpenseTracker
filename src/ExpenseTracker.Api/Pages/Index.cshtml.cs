using ExpenseTracker.Core.DTOs;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages;

public class IndexModel : PageModel
{
    private readonly ITransactionRepository _transactions;
    private readonly IBudgetRepository _budgets;

    public IndexModel(ITransactionRepository transactions, IBudgetRepository budgets)
    {
        _transactions = transactions;
        _budgets = budgets;
    }

    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime From => new DateTime(Year, Month, 1);
    public DateTime To => From.AddMonths(1).AddDays(-1);
    public MonthlySummaryDto Summary { get; set; } = null!;

    public async Task OnGetAsync(int? month, int? year, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        Month = month ?? now.Month;
        Year = year ?? now.Year;
        var from = new DateTime(Year, Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var list = await _transactions.GetByDateRangeAsync(from, to, null, ct);
        var budgets = await _budgets.GetByMonthAsync(Year, Month, ct);

        var totalIncome = list.Where(t => t.IsIncome).Sum(t => t.Amount);
        var totalExpense = list.Where(t => !t.IsIncome).Sum(t => t.Amount);
        var byCategory = list
            .GroupBy(t => new { t.CategoryId, t.Category!.Name, t.IsIncome })
            .Select(g => new CategorySummaryDto { CategoryId = g.Key.CategoryId, CategoryName = g.Key.Name, Total = g.Sum(t => t.Amount), IsIncome = g.Key.IsIncome })
            .ToList();
        var expenseByCategory = list.Where(t => !t.IsIncome).GroupBy(t => t.CategoryId).ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        var budgetProgress = budgets.Select(b => new BudgetProgressDto
        {
            CategoryId = b.CategoryId,
            CategoryName = b.Category!.Name,
            Spent = expenseByCategory.GetValueOrDefault(b.CategoryId, 0),
            Limit = b.AmountLimit
        }).ToList();

        Summary = new MonthlySummaryDto
        {
            Year = Year,
            Month = Month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            ByCategory = byCategory,
            BudgetProgress = budgetProgress
        };
    }
}

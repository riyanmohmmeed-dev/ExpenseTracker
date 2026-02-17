using ExpenseTracker.Core.DTOs;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummaryController : ControllerBase
{
    private readonly ITransactionRepository _transactions;
    private readonly IBudgetRepository _budgets;

    public SummaryController(ITransactionRepository transactions, IBudgetRepository budgets)
    {
        _transactions = transactions;
        _budgets = budgets;
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<MonthlySummaryDto>> GetMonthly(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken ct)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var list = await _transactions.GetByDateRangeAsync(from, to, null, ct);
        var budgets = await _budgets.GetByMonthAsync(year, month, ct);

        var totalIncome = list.Where(t => t.IsIncome).Sum(t => t.Amount);
        var totalExpense = list.Where(t => !t.IsIncome).Sum(t => t.Amount);
        var byCategory = list
            .GroupBy(t => new { t.CategoryId, t.Category!.Name, t.IsIncome })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Total = g.Sum(t => t.Amount),
                IsIncome = g.Key.IsIncome
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var expenseByCategory = list.Where(t => !t.IsIncome).GroupBy(t => t.CategoryId).ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        var budgetProgress = budgets
            .Select(b => new BudgetProgressDto
            {
                CategoryId = b.CategoryId,
                CategoryName = b.Category!.Name,
                Spent = expenseByCategory.GetValueOrDefault(b.CategoryId, 0),
                Limit = b.AmountLimit
            })
            .ToList();

        var summary = new MonthlySummaryDto
        {
            Year = year,
            Month = month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            ByCategory = byCategory,
            BudgetProgress = budgetProgress
        };
        return Ok(summary);
    }
}

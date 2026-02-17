using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Budgets;

public class IndexModel : PageModel
{
    private readonly IBudgetRepository _budgets;

    public IndexModel(IBudgetRepository budgets) => _budgets = budgets;

    public int Month { get; set; }
    public int Year { get; set; }
    public IReadOnlyList<Core.Entities.Budget> Budgets { get; set; } = null!;

    public async Task OnGetAsync(int? month, int? year, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        Month = month ?? now.Month;
        Year = year ?? now.Year;
        Budgets = await _budgets.GetByMonthAsync(Year, Month, ct);
    }
}

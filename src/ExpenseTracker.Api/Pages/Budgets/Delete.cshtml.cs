using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Budgets;

public class DeleteModel : PageModel
{
    private readonly IBudgetRepository _budgets;

    public DeleteModel(IBudgetRepository budgets) => _budgets = budgets;

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken ct)
    {
        var b = await _budgets.GetByIdAsync(id, ct);
        if (b == null) return NotFound();
        await _budgets.DeleteAsync(b, ct);
        return RedirectToPage("Index", new { month = b.Month, year = b.Year });
    }
}

using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Recurring;

public class IndexModel : PageModel
{
    private readonly IRecurringTransactionRepository _recurring;
    private readonly ITransactionRepository _transactions;

    public IndexModel(IRecurringTransactionRepository recurring, ITransactionRepository transactions)
    {
        _recurring = recurring;
        _transactions = transactions;
    }

    public IReadOnlyList<Core.Entities.RecurringTransaction> Recurring { get; set; } = null!;
    public string? Message { get; set; }

    public async Task OnGetAsync(CancellationToken ct) => Recurring = await _recurring.GetAllActiveAsync(ct);

    public async Task<IActionResult> OnPostGenerateAsync(int id, CancellationToken ct)
    {
        var r = await _recurring.GetByIdAsync(id, ct);
        if (r == null) return NotFound();
        var today = DateTime.UtcNow.Date;
        var generated = 0;
        var next = r.NextDueDate.Date;
        while (next <= today)
        {
            var existing = await _transactions.GetByDateRangeAsync(next, next, r.CategoryId, ct);
            var alreadyHas = existing.Any(t => t.Amount == r.Amount && t.IsIncome == r.IsIncome);
            if (!alreadyHas)
            {
                await _transactions.AddAsync(new Transaction
                {
                    Amount = r.Amount,
                    IsIncome = r.IsIncome,
                    Note = r.Note ?? "Recurring",
                    Date = next,
                    CategoryId = r.CategoryId,
                    CreatedAt = DateTime.UtcNow
                }, ct);
                generated++;
            }
            next = r.Frequency == RecurrenceFrequency.Monthly ? next.AddMonths(1) : next.AddDays(7);
        }
        r.NextDueDate = next;
        await _recurring.UpdateAsync(r, ct);
        TempData["Message"] = $"Generated {generated} transaction(s). Next due: {next:yyyy-MM-dd}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(int id, CancellationToken ct)
    {
        var r = await _recurring.GetByIdAsync(id, ct);
        if (r == null) return NotFound();
        r.IsActive = false;
        await _recurring.UpdateAsync(r, ct);
        TempData["Message"] = "Recurring item deactivated.";
        return RedirectToPage();
    }
}

using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Transactions;

public class DeleteModel : PageModel
{
    private readonly ITransactionRepository _transactions;

    public DeleteModel(ITransactionRepository transactions) => _transactions = transactions;

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken ct)
    {
        var t = await _transactions.GetByIdAsync(id, ct);
        if (t == null) return NotFound();
        await _transactions.DeleteAsync(t, ct);
        return RedirectToPage("Index");
    }
}

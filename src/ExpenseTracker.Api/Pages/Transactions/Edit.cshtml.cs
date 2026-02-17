using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Api.Pages.Transactions;

public class EditModel : PageModel
{
    private readonly ITransactionRepository _transactions;
    private readonly ICategoryRepository _categories;

    public EditModel(ITransactionRepository transactions, ICategoryRepository categories)
    {
        _transactions = transactions;
        _categories = categories;
    }

    [BindProperty] public int Id { get; set; }
    [BindProperty] public decimal Amount { get; set; }
    [BindProperty] public bool IsIncome { get; set; }
    [BindProperty] public int CategoryId { get; set; }
    [BindProperty] public DateTime Date { get; set; }
    [BindProperty] public string? Note { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var t = await _transactions.GetByIdAsync(id, ct);
        if (t == null) return NotFound();
        Id = t.Id; Amount = t.Amount; IsIncome = t.IsIncome; CategoryId = t.CategoryId; Date = t.Date; Note = t.Note;
        var list = await _categories.GetAllAsync(ct);
        CategoryOptions = list.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var t = await _transactions.GetByIdAsync(Id, ct);
        if (t == null) return NotFound();
        t.Amount = Amount; t.IsIncome = IsIncome; t.CategoryId = CategoryId; t.Date = Date; t.Note = Note;
        await _transactions.UpdateAsync(t, ct);
        return RedirectToPage("Index");
    }
}

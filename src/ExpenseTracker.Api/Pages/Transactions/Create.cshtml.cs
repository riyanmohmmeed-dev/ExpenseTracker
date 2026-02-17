using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Api.Pages.Transactions;

public class CreateModel : PageModel
{
    private readonly ITransactionRepository _transactions;
    private readonly ICategoryRepository _categories;

    public CreateModel(ITransactionRepository transactions, ICategoryRepository categories)
    {
        _transactions = transactions;
        _categories = categories;
    }

    [BindProperty] public decimal Amount { get; set; }
    [BindProperty] public bool IsIncome { get; set; }
    [BindProperty] public int CategoryId { get; set; }
    [BindProperty] public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    [BindProperty] public string? Note { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = null!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        var list = await _categories.GetAllAsync(ct);
        CategoryOptions = list.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(CategoryId, ct);
        if (category == null) { ModelState.AddModelError("CategoryId", "Invalid category."); await OnGetAsync(ct); return Page(); }
        var t = new Transaction { Amount = Amount, IsIncome = IsIncome, Note = Note, Date = Date, CategoryId = CategoryId, CreatedAt = DateTime.UtcNow };
        await _transactions.AddAsync(t, ct);
        return RedirectToPage("Index");
    }
}

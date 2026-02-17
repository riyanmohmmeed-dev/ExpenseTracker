using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Api.Pages.Recurring;

public class CreateModel : PageModel
{
    private readonly IRecurringTransactionRepository _recurring;
    private readonly ICategoryRepository _categories;

    public CreateModel(IRecurringTransactionRepository recurring, ICategoryRepository categories)
    {
        _recurring = recurring;
        _categories = categories;
    }

    [BindProperty] public int CategoryId { get; set; }
    [BindProperty] public decimal Amount { get; set; }
    [BindProperty] public bool IsIncome { get; set; }
    [BindProperty] public RecurrenceFrequency Frequency { get; set; }
    [BindProperty] public DateTime NextDueDate { get; set; }
    [BindProperty] public string? Note { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = null!;
    public List<SelectListItem> FrequencyOptions { get; set; } = null!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        NextDueDate = DateTime.UtcNow.Date;
        var list = await _categories.GetAllAsync(ct);
        CategoryOptions = list.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
        FrequencyOptions = Enum.GetValues<RecurrenceFrequency>().Select(f => new SelectListItem(f.ToString(), ((int)f).ToString())).ToList();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(CategoryId, ct);
        if (category == null) { ModelState.AddModelError("", "Category not found."); await OnGetAsync(ct); return Page(); }
        var r = new RecurringTransaction
        {
            Amount = Amount,
            IsIncome = IsIncome,
            Note = Note,
            CategoryId = CategoryId,
            Frequency = Frequency,
            NextDueDate = NextDueDate.Date,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _recurring.AddAsync(r, ct);
        return RedirectToPage("Index");
    }
}

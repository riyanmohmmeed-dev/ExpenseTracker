using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Api.Pages.Budgets;

public class CreateModel : PageModel
{
    private readonly IBudgetRepository _budgets;
    private readonly ICategoryRepository _categories;

    public CreateModel(IBudgetRepository budgets, ICategoryRepository categories)
    {
        _budgets = budgets;
        _categories = categories;
    }

    [BindProperty] public int CategoryId { get; set; }
    [BindProperty] public decimal AmountLimit { get; set; }
    [BindProperty] public int Year { get; set; }
    [BindProperty] public int Month { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = null!;

    public async Task OnGetAsync(int? year, int? month, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        Year = year ?? now.Year;
        Month = month ?? now.Month;
        var list = await _categories.GetAllAsync(ct);
        CategoryOptions = list.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var existing = await _budgets.GetByCategoryAndMonthAsync(CategoryId, Year, Month, ct);
        if (existing != null) { ModelState.AddModelError("", "Budget already exists for this category and month."); await OnGetAsync(Year, Month, ct); return Page(); }
        var b = new Budget { CategoryId = CategoryId, Year = Year, Month = Month, AmountLimit = AmountLimit };
        await _budgets.AddAsync(b, ct);
        return RedirectToPage("Index", new { month = Month, year = Year });
    }
}

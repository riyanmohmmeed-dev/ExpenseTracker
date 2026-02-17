using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly ICategoryRepository _categories;

    public CreateModel(ICategoryRepository categories) => _categories = categories;

    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string? Icon { get; set; }
    [BindProperty] public string? ColorHex { get; set; }

    public IActionResult OnGet() { ColorHex ??= "#0d6efd"; return Page(); }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var c = new Category { Name = Name.Trim(), Icon = Icon, ColorHex = ColorHex };
        await _categories.AddAsync(c, ct);
        return RedirectToPage("Index");
    }
}

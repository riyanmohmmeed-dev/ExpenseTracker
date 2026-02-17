using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ICategoryRepository _categories;

    public IndexModel(ICategoryRepository categories) => _categories = categories;

    public IReadOnlyList<Core.Entities.Category> Categories { get; set; } = null!;

    public async Task OnGetAsync(CancellationToken ct) => Categories = await _categories.GetAllAsync(ct);
}

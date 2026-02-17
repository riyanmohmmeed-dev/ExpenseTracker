using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Api.Pages.Transactions;

public class IndexModel : PageModel
{
    private readonly ITransactionRepository _transactions;
    private readonly ICategoryRepository _categories;

    public IndexModel(ITransactionRepository transactions, ICategoryRepository categories)
    {
        _transactions = transactions;
        _categories = categories;
    }

    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int? CategoryId { get; set; }
    public IReadOnlyList<Core.Entities.Transaction> Transactions { get; set; } = null!;
    public IReadOnlyList<Core.Entities.Category> Categories { get; set; } = null!;

    public async Task OnGetAsync(DateTime? from, DateTime? to, int? categoryId, CancellationToken ct)
    {
        var now = DateTime.UtcNow.Date;
        From = from ?? now.AddMonths(-1);
        To = to ?? now;
        CategoryId = categoryId;
        Categories = await _categories.GetAllAsync(ct);
        Transactions = await _transactions.GetByDateRangeAsync(From, To, categoryId, ct);
    }
}

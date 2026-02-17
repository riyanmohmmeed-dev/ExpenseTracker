using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _db;

    public BudgetRepository(AppDbContext db) => _db = db;

    public async Task<Budget?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<Budget>> GetByMonthAsync(int year, int month, CancellationToken ct = default) =>
        await _db.Budgets.Include(b => b.Category)
            .Where(b => b.Year == year && b.Month == month)
            .OrderBy(b => b.Category!.Name)
            .ToListAsync(ct);

    public async Task<Budget?> GetByCategoryAndMonthAsync(int categoryId, int year, int month, CancellationToken ct = default) =>
        await _db.Budgets.Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.CategoryId == categoryId && b.Year == year && b.Month == month, ct);

    public async Task<Budget> AddAsync(Budget budget, CancellationToken ct = default)
    {
        _db.Budgets.Add(budget);
        await _db.SaveChangesAsync(ct);
        return budget;
    }

    public async Task UpdateAsync(Budget budget, CancellationToken ct = default)
    {
        _db.Budgets.Update(budget);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Budget budget, CancellationToken ct = default)
    {
        _db.Budgets.Remove(budget);
        await _db.SaveChangesAsync(ct);
    }
}

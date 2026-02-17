using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class RecurringTransactionRepository : IRecurringTransactionRepository
{
    private readonly AppDbContext _db;

    public RecurringTransactionRepository(AppDbContext db) => _db = db;

    public async Task<RecurringTransaction?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.RecurringTransactions.Include(r => r.Category).FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<RecurringTransaction>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.RecurringTransactions.Include(r => r.Category)
            .Where(r => r.IsActive)
            .OrderBy(r => r.NextDueDate)
            .ToListAsync(ct);

    public async Task<RecurringTransaction> AddAsync(RecurringTransaction recurring, CancellationToken ct = default)
    {
        _db.RecurringTransactions.Add(recurring);
        await _db.SaveChangesAsync(ct);
        return recurring;
    }

    public async Task UpdateAsync(RecurringTransaction recurring, CancellationToken ct = default)
    {
        _db.RecurringTransactions.Update(recurring);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(RecurringTransaction recurring, CancellationToken ct = default)
    {
        _db.RecurringTransactions.Remove(recurring);
        await _db.SaveChangesAsync(ct);
    }
}

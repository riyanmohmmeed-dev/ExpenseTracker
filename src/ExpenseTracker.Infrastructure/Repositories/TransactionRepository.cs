using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db) => _db = db;

    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Transactions.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to, int? categoryId, CancellationToken ct = default)
    {
        var query = _db.Transactions
            .Include(t => t.Category)
            .Where(t => t.Date >= from && t.Date <= to);
        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);
        return await query.OrderByDescending(t => t.Date).ToListAsync(ct);
    }

    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync(ct);
        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Update(transaction);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Remove(transaction);
        await _db.SaveChangesAsync(ct);
    }
}

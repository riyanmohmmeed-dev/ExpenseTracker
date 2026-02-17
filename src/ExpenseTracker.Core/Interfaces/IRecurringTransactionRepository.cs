using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IRecurringTransactionRepository
{
    Task<RecurringTransaction?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<RecurringTransaction>> GetAllActiveAsync(CancellationToken ct = default);
    Task<RecurringTransaction> AddAsync(RecurringTransaction recurring, CancellationToken ct = default);
    Task UpdateAsync(RecurringTransaction recurring, CancellationToken ct = default);
    Task DeleteAsync(RecurringTransaction recurring, CancellationToken ct = default);
}

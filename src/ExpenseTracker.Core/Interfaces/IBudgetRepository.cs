using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Budget>> GetByMonthAsync(int year, int month, CancellationToken ct = default);
    Task<Budget?> GetByCategoryAndMonthAsync(int categoryId, int year, int month, CancellationToken ct = default);
    Task<Budget> AddAsync(Budget budget, CancellationToken ct = default);
    Task UpdateAsync(Budget budget, CancellationToken ct = default);
    Task DeleteAsync(Budget budget, CancellationToken ct = default);
}

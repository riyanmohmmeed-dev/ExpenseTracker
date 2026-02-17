using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurringController : ControllerBase
{
    private readonly IRecurringTransactionRepository _recurring;
    private readonly ICategoryRepository _categories;
    private readonly ITransactionRepository _transactions;

    public RecurringController(
        IRecurringTransactionRepository recurring,
        ICategoryRepository categories,
        ITransactionRepository transactions)
    {
        _recurring = recurring;
        _categories = categories;
        _transactions = transactions;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecurringTransaction>>> GetAll(CancellationToken ct)
    {
        var list = await _recurring.GetAllActiveAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecurringTransaction>> GetById(int id, CancellationToken ct)
    {
        var r = await _recurring.GetByIdAsync(id, ct);
        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public async Task<ActionResult<RecurringTransaction>> Create([FromBody] CreateRecurringRequest request, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(request.CategoryId, ct);
        if (category == null) return BadRequest("Category not found.");
        var recurring = new RecurringTransaction
        {
            Amount = request.Amount,
            IsIncome = request.IsIncome,
            Note = request.Note,
            CategoryId = request.CategoryId,
            Frequency = request.Frequency,
            NextDueDate = request.NextDueDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _recurring.AddAsync(recurring, ct);
        return CreatedAtAction(nameof(GetById), new { id = recurring.Id }, recurring);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateRecurringRequest request, CancellationToken ct)
    {
        var r = await _recurring.GetByIdAsync(id, ct);
        if (r == null) return NotFound();
        if (request.Amount.HasValue) r.Amount = request.Amount.Value;
        if (request.IsIncome.HasValue) r.IsIncome = request.IsIncome.Value;
        if (request.Note != null) r.Note = request.Note;
        if (request.NextDueDate.HasValue) r.NextDueDate = request.NextDueDate.Value;
        if (request.IsActive.HasValue) r.IsActive = request.IsActive.Value;
        await _recurring.UpdateAsync(r, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var r = await _recurring.GetByIdAsync(id, ct);
        if (r == null) return NotFound();
        await _recurring.DeleteAsync(r, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/generate")]
    public async Task<ActionResult> GenerateUpToToday(int id, CancellationToken ct)
    {
        var r = await _recurring.GetByIdAsync(id, ct);
        if (r == null) return NotFound();
        var today = DateTime.UtcNow.Date;
        var generated = 0;
        var next = r.NextDueDate.Date;
        while (next <= today)
        {
            var existing = await _transactions.GetByDateRangeAsync(next, next, r.CategoryId, ct);
            var alreadyHas = existing.Any(t => t.Amount == r.Amount && t.IsIncome == r.IsIncome);
            if (!alreadyHas)
            {
                await _transactions.AddAsync(new Transaction
                {
                    Amount = r.Amount,
                    IsIncome = r.IsIncome,
                    Note = r.Note ?? "Recurring",
                    Date = next,
                    CategoryId = r.CategoryId,
                    CreatedAt = DateTime.UtcNow
                }, ct);
                generated++;
            }
            next = r.Frequency == RecurrenceFrequency.Monthly ? next.AddMonths(1) : next.AddDays(7);
        }
        r.NextDueDate = next;
        await _recurring.UpdateAsync(r, ct);
        return Ok(new { generated, nextDueDate = next });
    }
}

public record CreateRecurringRequest(decimal Amount, bool IsIncome, string? Note, int CategoryId, RecurrenceFrequency Frequency, DateTime NextDueDate);
public record UpdateRecurringRequest(decimal? Amount, bool? IsIncome, string? Note, DateTime? NextDueDate, bool? IsActive);

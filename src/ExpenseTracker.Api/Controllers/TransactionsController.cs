using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactions;
    private readonly ICategoryRepository _categories;

    public TransactionsController(ITransactionRepository transactions, ICategoryRepository categories)
    {
        _transactions = transactions;
        _categories = categories;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetByRange(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? categoryId,
        CancellationToken ct)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow.Date;
        var list = await _transactions.GetByDateRangeAsync(fromDate, toDate, categoryId, ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Transaction>> GetById(int id, CancellationToken ct)
    {
        var transaction = await _transactions.GetByIdAsync(id, ct);
        if (transaction == null) return NotFound();
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> Create([FromBody] CreateTransactionRequest request, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(request.CategoryId, ct);
        if (category == null) return BadRequest("Category not found.");
        var transaction = new Transaction
        {
            Amount = request.Amount,
            IsIncome = request.IsIncome,
            Note = request.Note,
            Date = request.Date,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
        await _transactions.AddAsync(transaction, ct);
        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTransactionRequest request, CancellationToken ct)
    {
        var transaction = await _transactions.GetByIdAsync(id, ct);
        if (transaction == null) return NotFound();
        if (request.CategoryId.HasValue)
        {
            var category = await _categories.GetByIdAsync(request.CategoryId.Value, ct);
            if (category == null) return BadRequest("Category not found.");
            transaction.CategoryId = request.CategoryId.Value;
        }
        if (request.Amount.HasValue) transaction.Amount = request.Amount.Value;
        if (request.IsIncome.HasValue) transaction.IsIncome = request.IsIncome.Value;
        if (request.Note != null) transaction.Note = request.Note;
        if (request.Date.HasValue) transaction.Date = request.Date.Value;
        await _transactions.UpdateAsync(transaction, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var transaction = await _transactions.GetByIdAsync(id, ct);
        if (transaction == null) return NotFound();
        await _transactions.DeleteAsync(transaction, ct);
        return NoContent();
    }
}

public record CreateTransactionRequest(decimal Amount, bool IsIncome, string? Note, DateTime Date, int CategoryId);
public record UpdateTransactionRequest(decimal? Amount, bool? IsIncome, string? Note, DateTime? Date, int? CategoryId);

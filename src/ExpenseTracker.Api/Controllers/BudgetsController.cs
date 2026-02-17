using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetRepository _budgets;
    private readonly ICategoryRepository _categories;

    public BudgetsController(IBudgetRepository budgets, ICategoryRepository categories)
    {
        _budgets = budgets;
        _categories = categories;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Budget>>> GetByMonth([FromQuery] int year, [FromQuery] int month, CancellationToken ct)
    {
        var list = await _budgets.GetByMonthAsync(year, month, ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Budget>> GetById(int id, CancellationToken ct)
    {
        var budget = await _budgets.GetByIdAsync(id, ct);
        if (budget == null) return NotFound();
        return Ok(budget);
    }

    [HttpPost]
    public async Task<ActionResult<Budget>> Create([FromBody] CreateBudgetRequest request, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(request.CategoryId, ct);
        if (category == null) return BadRequest("Category not found.");
        var existing = await _budgets.GetByCategoryAndMonthAsync(request.CategoryId, request.Year, request.Month, ct);
        if (existing != null) return BadRequest("Budget already exists for this category and month.");
        var budget = new Budget
        {
            CategoryId = request.CategoryId,
            Year = request.Year,
            Month = request.Month,
            AmountLimit = request.AmountLimit
        };
        await _budgets.AddAsync(budget, ct);
        return CreatedAtAction(nameof(GetById), new { id = budget.Id }, budget);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateBudgetRequest request, CancellationToken ct)
    {
        var budget = await _budgets.GetByIdAsync(id, ct);
        if (budget == null) return NotFound();
        if (request.AmountLimit.HasValue) budget.AmountLimit = request.AmountLimit.Value;
        await _budgets.UpdateAsync(budget, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var budget = await _budgets.GetByIdAsync(id, ct);
        if (budget == null) return NotFound();
        await _budgets.DeleteAsync(budget, ct);
        return NoContent();
    }
}

public record CreateBudgetRequest(int CategoryId, int Year, int Month, decimal AmountLimit);
public record UpdateBudgetRequest(decimal? AmountLimit);

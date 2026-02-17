using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categories;

    public CategoriesController(ICategoryRepository categories) => _categories = categories;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll(CancellationToken ct)
    {
        var list = await _categories.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Category>> GetById(int id, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(id, ct);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var category = new Category
        {
            Name = request.Name,
            Icon = request.Icon,
            ColorHex = request.ColorHex
        };
        await _categories.AddAsync(category, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(id, ct);
        if (category == null) return NotFound();
        category.Name = request.Name ?? category.Name;
        category.Icon = request.Icon ?? category.Icon;
        category.ColorHex = request.ColorHex ?? category.ColorHex;
        await _categories.UpdateAsync(category, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(id, ct);
        if (category == null) return NotFound();
        await _categories.DeleteAsync(category, ct);
        return NoContent();
    }
}

public record CreateCategoryRequest(string Name, string? Icon, string? ColorHex);
public record UpdateCategoryRequest(string? Name, string? Icon, string? ColorHex);

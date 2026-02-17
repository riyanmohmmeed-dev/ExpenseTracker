using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly ITransactionRepository _transactions;

    public ExportController(ITransactionRepository transactions) => _transactions = transactions;

    [HttpGet("csv")]
    public async Task<IActionResult> GetCsv(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow.Date;
        var list = await _transactions.GetByDateRangeAsync(fromDate, toDate, null, ct);

        var sb = new StringBuilder();
        sb.AppendLine("Date,Category,Amount,Type,Note");
        foreach (var t in list.OrderBy(x => x.Date))
        {
            var note = (t.Note ?? "").Replace("\"", "\"\"");
            sb.AppendLine($"{t.Date:yyyy-MM-dd},\"{t.Category?.Name ?? ""}\",{t.Amount:F2},{(t.IsIncome ? "Income" : "Expense")},\"{note}\"");
        }
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var fileName = $"expenses_{fromDate:yyyy-MM-dd}_to_{toDate:yyyy-MM-dd}.csv";
        return File(bytes, "text/csv", fileName);
    }
}

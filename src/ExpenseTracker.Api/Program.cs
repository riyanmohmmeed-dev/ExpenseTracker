using ExpenseTracker.Core.Entities;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ExpenseTracker API", Version = "v1" });
});
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Categories.AnyAsync())
    {
        db.Categories.AddRange(
            new Category { Name = "Food", Icon = "🍔", ColorHex = "#ef4444" },
            new Category { Name = "Transport", Icon = "🚗", ColorHex = "#3b82f6" },
            new Category { Name = "Salary", Icon = "💰", ColorHex = "#22c55e" },
            new Category { Name = "Shopping", Icon = "🛒", ColorHex = "#f59e0b" }
        );
        await db.SaveChangesAsync();
    }
}

app.UseCors();
app.UseStaticFiles();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpenseTracker v1"));
app.MapControllers();
app.MapRazorPages();

app.Run();

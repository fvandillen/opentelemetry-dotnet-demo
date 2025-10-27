using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.OrderingApi.Database;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<OrderingContext>(connectionName: "ordering");

var app = builder.Build();
app.UseServiceDefaults();

app.MapGet("/test", async (OrderingContext db) =>
{
    var orders = await db.Orders.ToListAsync();
    return orders;
});

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<OrderingContext>().Database.EnsureCreatedAsync();
}

app.Run();
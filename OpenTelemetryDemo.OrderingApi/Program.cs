using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Domain;
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

app.MapPost("/order", async (OrderingContext db, Order order) =>
{
    await db.Orders.AddAsync(order);
    await db.SaveChangesAsync();
    // TODO: send service bus message.
    return Results.Ok(order);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderingContext>();
    await db.Database.EnsureCreatedAsync();

    await db.Orders.AddAsync(
        new Order()
        {
            Id = Guid.NewGuid(), 
            OrderLines = [new OrderLine { ProductName = "Blue hat", Quantity = 7 }]
        });
    await db.SaveChangesAsync();
}

app.Run();
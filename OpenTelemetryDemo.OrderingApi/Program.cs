using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Domain;
using OpenTelemetryDemo.OrderingApi;
using OpenTelemetryDemo.OrderingApi.Database;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<OrderingContext>(connectionName: "ordering");
builder.AddCustomMeter<OrderingMetrics>();

var app = builder.Build();
app.UseServiceDefaults();

app.MapGet("/test", async ([FromServices] OrderingContext db, [FromServices] OrderingMetrics metrics) =>
{
    var orders = await db.Orders.ToListAsync();
    return orders;
});

app.MapPost("/order", async ([FromServices] OrderingContext db, [FromServices] OrderingMetrics metrics, Order order) =>
{
    await db.Orders.AddAsync(order);
    await db.SaveChangesAsync();
    
    foreach(var productLine in order.OrderLines)
    {
        metrics.RecordOrderPlaced(productLine.ProductName, productLine.Quantity);
    }
    
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
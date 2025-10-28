using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Domain;
using OpenTelemetryDemo.Domain.DataGenerators;
using OpenTelemetryDemo.OrderingApi;
using OpenTelemetryDemo.OrderingApi.Database;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<OrderingContext>(connectionName: "ordering");
builder.AddCustomMeter<OrderingMetrics>();
builder.Services.AddSingleton<OrderFaker>();

var app = builder.Build();
app.UseServiceDefaults();

app.MapGet("/test", async (OrderingContext db, OrderingMetrics metrics) =>
{
    var orders = await db.Orders.ToListAsync();
    return orders;
});

app.MapPost("/order", async (OrderingContext db, OrderingMetrics metrics, Order order) =>
{
    await db.Orders.AddAsync(order);
    await db.SaveChangesAsync();
    
    foreach(var productLine in order.OrderLines)
    {
        metrics.RecordOrderPlaced(productLine.ProductName, productLine.Color, productLine.Quantity);
    }
    
    // TODO: send service bus message.
    return Results.Ok(order);
});

app.MapPost("/order/generate", async (OrderingContext db, OrderingMetrics metrics, OrderFaker faker) =>
{
    var order = faker.Generate(1).Single();
    await db.Orders.AddAsync(order);
    await db.SaveChangesAsync();
    
    foreach(var productLine in order.OrderLines)
    {
        metrics.RecordOrderPlaced(productLine.ProductName, productLine.Color, productLine.Quantity);
    }
    
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
            OrderLines = [new OrderLine { ProductName = "Hat", Quantity = 7, Color = "Blue"}]
        });
    await db.SaveChangesAsync();
}

app.Run();
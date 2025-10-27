using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Domain;

namespace OpenTelemetryDemo.OrderingApi.Database;

public class OrderingContext(DbContextOptions<OrderingContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
}
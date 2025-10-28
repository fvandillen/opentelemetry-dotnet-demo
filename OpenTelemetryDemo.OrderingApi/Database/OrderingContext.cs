using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Domain;

namespace OpenTelemetryDemo.OrderingApi.Database;

public class OrderingContext(DbContextOptions<OrderingContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(x => x.OwnsMany(o => o.OrderLines));
    }
}
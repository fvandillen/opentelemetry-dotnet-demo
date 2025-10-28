using Bogus;

namespace OpenTelemetryDemo.Domain.DataGenerators;

public sealed class OrderFaker : Faker<Order>
{
    public OrderFaker()
    {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.OrderLines, f => GetOrderLineFaker().Generate(f.Random.Int(1, 5)));
    }

    private Faker<OrderLine> GetOrderLineFaker()
    {
        return new Faker<OrderLine>()
            .RuleFor(x => x.Quantity, f => f.Random.Int(1, 100))
            .RuleFor(x => x.ProductName, f => f.Commerce.Product())
            .RuleFor(x => x.Color, f => f.Commerce.Color());
    }
}
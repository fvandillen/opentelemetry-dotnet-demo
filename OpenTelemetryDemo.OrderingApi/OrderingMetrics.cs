using System.Diagnostics.Metrics;

namespace OpenTelemetryDemo.OrderingApi;

public class OrderingMetrics
{
    public const string MeterName = "OpenTelemetryDemo.OrderingApi.OrderingMetrics";
    private static Histogram<int>? _ordersPlacedCounter;
    
    public OrderingMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(new MeterOptions(MeterName) { Version = "1.0.0" });
        _ordersPlacedCounter = meter.CreateHistogram<int>("orders_placed", description: "Counts the number of orders placed");
    }

    public void SimpleRecord()
    {
        //_ordersPlacedCounter?.Add(1);
        _ordersPlacedCounter?.Record(1);
    }

    public void RecordOrderPlaced(string product, string color, int quantity)
    {
        _ordersPlacedCounter?.Record(1, new[]
        {
            new KeyValuePair<string, object?>("product", product),
            new KeyValuePair<string, object?>("color", color),
            new KeyValuePair<string, object?>("quantity", quantity)
        });
    }
}
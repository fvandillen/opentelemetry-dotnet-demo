namespace OpenTelemetryDemo.Domain;

public class OrderLine
{
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
}
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBusClient(connectionName: "orders");

var app = builder.Build();
app.UseServiceDefaults();

using (var scope = app.Services.CreateScope())
{
    var serviceBusClient = scope.ServiceProvider.GetRequiredService<ServiceBusClient>();
    var receiver = serviceBusClient.CreateProcessor("orders");
    receiver.ProcessMessageAsync += async args =>
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"Received message: {body}");
        await args.CompleteMessageAsync(args.Message);
    };

    receiver.ProcessErrorAsync += async args =>
    {
        Console.WriteLine($"Received error: {args.Exception}");
    };
    
    await receiver.StartProcessingAsync();
}
app.Run();
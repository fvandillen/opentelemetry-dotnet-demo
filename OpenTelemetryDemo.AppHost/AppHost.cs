var builder = DistributedApplication.CreateBuilder(args);

var inventoryApi = builder.AddProject<Projects.OpenTelemetryDemo_InventoryApi>("inventory-api");

builder.AddProject<Projects.OpenTelemetryDemo_OrderingApi>("ordering-api").WithReference(inventoryApi);

builder.Build().Run();

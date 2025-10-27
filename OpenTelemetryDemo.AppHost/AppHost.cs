var builder = DistributedApplication.CreateBuilder(args);

var inventoryApi = builder.AddProject<Projects.OpenTelemetryDemo_InventoryApi>("inventory-api");

var orderingDbServer = builder.AddPostgres("ordering-db");
var orderingDb = orderingDbServer.AddDatabase("ordering");

builder.AddProject<Projects.OpenTelemetryDemo_OrderingApi>("ordering-api")
    .WithReference(inventoryApi)
    .WaitFor(orderingDb)
    .WithReference(orderingDb);

builder.Build().Run();

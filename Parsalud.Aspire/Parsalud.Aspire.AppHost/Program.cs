var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Parsalud>("landing", "https landing")
    .WithEnvironment("UseDashboard", "false");

builder.AddProject<Projects.Parsalud>("dashboard", "https dashboard")
    .WithEnvironment("UseDashboard", "true");

await builder.Build().RunAsync();

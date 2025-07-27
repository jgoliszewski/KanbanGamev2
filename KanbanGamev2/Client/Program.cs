using KanbanGamev2.Client;
using KanbanGamev2.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register our client services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Register drag and drop service
builder.Services.AddScoped<IDragDropService, DragDropService>();

// Register SignalR service
builder.Services.AddScoped<ISignalRService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new SignalRService(httpClient.BaseAddress?.ToString() ?? "https://localhost:7001/");
});

// Register game state service
builder.Services.AddScoped<KanbanGamev2.Shared.Services.IGameStateService, GameStateService>();

// Register game state manager (singleton for persistence)
builder.Services.AddSingleton<IGameStateManager, GameStateManager>();

// Register work simulation service
builder.Services.AddScoped<IWorkSimulationService, WorkSimulationService>();

// Register global loader service
builder.Services.AddScoped<IGlobalLoaderService, GlobalLoaderService>();

await builder.Build().RunAsync();

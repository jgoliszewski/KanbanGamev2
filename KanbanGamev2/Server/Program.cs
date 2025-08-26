using KanbanGamev2.Server.Services;
using KanbanGamev2.Server.SignalR;
using KanbanGamev2.Shared.Services;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register our services as singletons (like the working project)
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddSingleton<IFeatureService, FeatureService>();
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<IGameStateService, GameStateService>();
builder.Services.AddSingleton<IGameRestartService, GameRestartService>();

// Add SignalR (for future real-time updates)
builder.Services.AddSignalR();

// Add response compression
builder.Services.AddResponseCompression(
    options =>
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" }
        )
);

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapHub<GameHub>("/gamehub");
app.MapHub<NotificationHub>("/notificationhub");
app.MapFallbackToFile("index.html");

app.Run();

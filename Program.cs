using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskTracker.Data;
using TaskTracker.Endpoints;
using TaskTracker.Services;
using MyTaskFactory = TaskTracker.Services.TaskFactory;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();                                                          // Adds OpenAPI/Swagger services to the application for API documentation and testing

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddDbContext<AppDbContext>(options =>                                  // Registers the AppDbContext with the dependency injection container,
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // configuring it to use SQL Server with a connection string from the configuration

// New task service is created with each HTTP request,
// ensuring that it has a fresh instance of the database context for each operation
builder.Services.AddScoped<ITaskService, TaskService>();

// One TaskFacory instance is shared for the apps lifetime
builder.Services.AddSingleton<MyTaskFactory>();

// Registers the OverdueTaskWorker as a hosted service, which will run in the background independently of the HTTP request handling
builder.Services.AddHostedService<OverdueTaskWorker>();

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();                                                              // Adds middleware to redirect HTTP requests to HTTPS,
                                                                                        // ensuring secure communication between the client and server
app.MapTaskEndpoints();

app.Run();                                                                              // Starts the application and begins listening for incoming HTTP requests,
                                                                                        // effectively running the web server and making the API available to clients
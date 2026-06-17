using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Services;
using MyTaskFactory = TaskTracker.Services.TaskFactory;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();                                                          // Adds OpenAPI/Swagger services to the application for API documentation and testing

builder.Services.AddDbContext<AppDbContext>(options =>                                  // Registers the AppDbContext with the dependency injection container,
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // configuring it to use SQL Server with a connection string from the configuration

// New task service is created with each HTTP request,
// ensuring that it has a fresh instance of the database context for each operation
builder.Services.AddScoped<ITaskService, TaskService>();

// One TaskFacory instance is shared for the apps lifetime
builder.Services.AddSingleton<MyTaskFactory>();

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
    app.MapOpenApi();                                                                   // Maps the OpenAPI/Swagger UI to the application,
                                                                                        // allowing interactions with the API documentation and test API endpoints during development
}

app.UseHttpsRedirection();                                                              // Adds middleware to redirect HTTP requests to HTTPS,
                                                                                        // ensuring secure communication between the client and server

app.Run();                                                                              // Starts the application and begins listening for incoming HTTP requests,
                                                                                        // effectively running the web server and making the API available to clients
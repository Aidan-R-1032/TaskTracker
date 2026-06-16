using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();                                                          // Adds OpenAPI/Swagger services to the application for API documentation and testing

builder.Services.AddDbContext<AppDbContext>(options =>                                  // Registers the AppDbContext with the dependency injection container,
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // configuring it to use SQL Server with a connection string from the configuration

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();                  // Creates a scope to resolve the AppDbContext service
    
    db.Database.MigrateAsync().Wait();                                                  // Applies any pending migrations to the database, ensuring that the
                                                                                        // database schema is up to date with the application's data model
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
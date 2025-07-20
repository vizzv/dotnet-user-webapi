using Microsoft.EntityFrameworkCore;
using UserCrudApi.Data;
using UserCrudApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MigrationService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await migrationService.EnsureDatabaseCreatedAsync();
        logger.LogInformation("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed. Application will continue but may not work correctly.");
        // Don't stop the application, just log the error
    }
}

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add a health check endpoint
app.MapGet("/health", () => "API is running!");

app.Run();

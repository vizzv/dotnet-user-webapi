using Microsoft.EntityFrameworkCore;
using UserCrudApi.Data;
using UserCrudApi.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("PG_HOST");
var database = Environment.GetEnvironmentVariable("PG_DATABASE");
var user = Environment.GetEnvironmentVariable("PG_USER");
var password = Environment.GetEnvironmentVariable("PG_PASSWORD");

var pgStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = host,
    Database = database,
    Username = user,
    Password = password,
};

string pgConnectionString = pgStringBuilder.ConnectionString;

// Add services to the container.
builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(pgConnectionString));

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

if(Environment.GetEnvironmentVariable("IS_DEVELOPMENT")== "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");

}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add a health check endpoint
app.MapGet("/health", () => "API is running!");

app.Run();

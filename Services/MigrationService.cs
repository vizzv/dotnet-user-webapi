using Microsoft.EntityFrameworkCore;
using UserCrudApi.Data;
using UserCrudApi.Migrations;

namespace UserCrudApi.Services
{
    public class MigrationService
    {
        private readonly UserContext _context;
        private readonly ILogger<MigrationService> _logger;

        public MigrationService(UserContext context, ILogger<MigrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task EnsureDatabaseCreatedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database migration...");

                // Ensure database exists
                await _context.Database.EnsureCreatedAsync();

                // Check if users table exists, if not create it
                var tableExists = await _context.Database.ExecuteSqlRawAsync(@"
                    SELECT 1 FROM information_schema.tables 
                    WHERE table_name = 'users' AND table_schema = 'public'
                ");

                if (tableExists == 0)
                {
                    await ManualMigrations.ApplyInitialMigration(_context);
                    _logger.LogInformation("Initial migration applied successfully.");
                }
                else
                {
                    _logger.LogInformation("Database already exists and is up to date.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while ensuring database creation.");
                throw;
            }
        }
    }
}


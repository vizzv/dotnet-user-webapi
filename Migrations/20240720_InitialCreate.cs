using Microsoft.EntityFrameworkCore;
using UserCrudApi.Data;

namespace UserCrudApi.Migrations
{
    public static class ManualMigrations
    {
        public static async Task ApplyInitialMigration(UserContext context)
        {
            // Create the users table
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) NOT NULL,
                    email VARCHAR(255) NOT NULL
                );
            ");
        }
    }
}

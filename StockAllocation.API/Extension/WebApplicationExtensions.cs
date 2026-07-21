using Microsoft.EntityFrameworkCore;
using StockAllocation.Infrastructure.Persistence;

namespace StockAllocation.API.Extension
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await db.Database.MigrateAsync();
            return app;
        }

    }
}

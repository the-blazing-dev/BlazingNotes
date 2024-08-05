using BlazingNotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazingNotes.Infrastructure.Hosting;

public static class DbMigrator
{
    public static async Task MigrateDbAsync(this IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<AppDb>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDb>>();
        var connectionString = migrator.Database.GetConnectionString();
        logger.LogInformation("Notes are stored here: " + connectionString);
        await migrator.Database.MigrateAsync();
    }
}
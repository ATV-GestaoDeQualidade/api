using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MinhaApi.Infraestrutura.Data;

public static class DatabaseMigrationExtensions
{
    public static async Task MigrateDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MinhaApiDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

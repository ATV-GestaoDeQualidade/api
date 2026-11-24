using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MinhaApi.Infraestrutura.Services;

internal static class SqliteConstraintErrors
{
    public static bool IsUniqueConstraint(DbUpdateException exception) =>
        exception.InnerException is SqliteException { SqliteErrorCode: 19 };
}

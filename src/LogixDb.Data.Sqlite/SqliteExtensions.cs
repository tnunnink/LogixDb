using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides extension methods for working with SQLite database types in the LogixDb context.
/// </summary>
internal static class SqliteExtensions
{
    /// <summary>
    /// Generates a SQLite connection string based on the provided <see cref="DbConnectionInfo"/>.
    /// </summary>
    /// <param name="info">The SQL connection information containing details like data source and authentication.</param>
    /// <returns>A SQLite connection string constructed from the specified <paramref name="info"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided <paramref name="info"/> is null.
    /// </exception>
    public static string ToConnectionString(this DbConnectionInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = info.Source,
            ForeignKeys = true,
            Pooling = false,
        };

        return builder.ToString();
    }

    /// <summary>
    /// Converts a .NET <see cref="Type"/> to its corresponding <see cref="SqliteType"/> representation.
    /// </summary>
    /// <param name="type">The .NET type to be converted to an equivalent SQLite type.</param>
    /// <returns>The corresponding <see cref="SqliteType"/> for the provided <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided <paramref name="type"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the provided <paramref name="type"/> is not supported for SQLite conversion.
    /// </exception>
    public static SqliteType ToSqliteType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type == typeof(bool) || type == typeof(byte) || type == typeof(short) || type == typeof(int) ||
            type == typeof(long))
            return SqliteType.Integer;

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            return SqliteType.Real;

        if (type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan) || type == typeof(Guid))
            return SqliteType.Text;

        if (type == typeof(byte[]))
            return SqliteType.Blob;

        throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported type for SQLite conversion.");
    }
}
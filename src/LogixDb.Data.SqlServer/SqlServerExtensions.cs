using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Provides extension methods for working with SQL Server database types in the LogixDb context.
/// </summary>
public static class SqlServerExtensions
{
    /// <summary>
    /// Converts the given <see cref="DbConnectionInfo"/> into a connection string for SQL Server.
    /// </summary>
    /// <param name="info">An instance of <see cref="DbConnectionInfo"/> that contains the connection details.</param>
    /// <param name="database">An optional database name to override the default catalog defined in <paramref name="info"/>.</param>
    /// <returns>A SQL Server connection string based on the provided connection information.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the authentication type specified in <paramref name="info"/> is not supported.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="info"/> is null.
    /// </exception>
    public static string ToConnectionString(this DbConnectionInfo info, string? database = null)
    {
        ArgumentNullException.ThrowIfNull(info);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = $"{info.Source},{info.Port}",
            InitialCatalog = database ?? info.Database,
            Encrypt = info.Encrypt,
            TrustServerCertificate = info.Trust,
            Pooling = false
        };

        if (info.User is not null)
        {
            // SQL Server authentication
            builder.UserID = info.User;
            builder.Password = info.Password;
            builder.IntegratedSecurity = false;
        }
        else
        {
            builder.IntegratedSecurity = true;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converts a .NET <see cref="Type"/> to its corresponding SQL Server type representation as a string.
    /// </summary>
    /// <param name="type">The .NET type to be converted to an equivalent SQL Server type.</param>
    /// <returns>The corresponding SQL Server type string for the provided <paramref name="type"/>.</returns>
    public static string ToSqlServerType(this Type type)
    {
        if (type == typeof(string)) return "NVARCHAR(MAX)";
        if (type == typeof(int)) return "INT";
        if (type == typeof(long)) return "BIGINT";
        if (type == typeof(bool)) return "BIT";
        if (type == typeof(DateTime)) return "DATETIME2";
        if (type == typeof(Guid)) return "UNIQUEIDENTIFIER";
        if (type == typeof(byte[])) return "VARBINARY(MAX)";
        if (type == typeof(float)) return "REAL";
        if (type == typeof(double)) return "FLOAT";
        return "NVARCHAR(MAX)";
    }
}

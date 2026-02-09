/*using System.Data;
using System.Data.SqlClient;
using JetBrains.Annotations;
using L5Sharp.Sql.Abstractions;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

namespace L5Sharp.Sql.Providers;

/// <summary>
/// Provides SQL Server database connection functionality by implementing the <see cref="IConnectionProvider"/> interface.
/// This class is designed to establish connections to a specified SQL Server database using provided connection parameters.
/// </summary>
[UsedImplicitly]
public class SqlServerConnectionProvider(string server, string database) : IConnectionProvider
{
    public async Task<IDbConnection> Connect(CancellationToken token)
    {
        var connectionString = BuildConnectionString(server, database);
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        return connection;
    }

    /// <summary>
    /// Builds the required connection string to our application database.
    /// </summary>
    private static string BuildConnectionString(string server, string database)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            //todo idk if we actually want to determine this externally from the CLI.
            // users may have db not in windows domain
            IntegratedSecurity = false,
            Encrypt = true,
            TrustServerCertificate = true,
            ConnectTimeout = 30
        };

        return builder.ConnectionString;
    }
}*/
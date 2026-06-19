using System.Data.Common;
using System.Reflection;
using LogixDb.Data.Abstractions;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Provides SQL Server-specific implementations of database provider functionality.
/// This class facilitates the creation and management of SQL Server connections,
/// resource-based script loading, and writer provisioning.
/// </summary>
public class SqlServerProvider(DbConnectionInfo connection) : IDbProvider
{
    /// <inheritdoc />
    public async Task<DbConnection> OpenConnection(CancellationToken token = default)
    {
        var sqlConnection = new SqlConnection(connection.ToConnectionString());
        await sqlConnection.OpenAsync(token);
        return sqlConnection;
    }

    /// <inheritdoc />
    public IDbWriter GetWriter()
    {
        return new SqlServerWriter(this);
    }

    /// <inheritdoc />
    public string GetScript(ScriptName scriptName)
    {
        return LoadResource($"Scripts.Queries.{scriptName}");
    }

    /// <inheritdoc />
    public string GetMergeScript(string tableName)
    {
        // Convert snake_case (tag_member) to PascalCase (TagMember)
        var parts = tableName.Split('_');
        var pascalName = string.Join("", parts.Select(p => char.ToUpper(p[0]) + p[1..]));

        // Build the resource name following the Merge{Table} convention
        var scriptName = $"Merge{pascalName}";

        return LoadResource($"Scripts.Merges.{scriptName}");
    }

    /// <summary>
    /// Loads an embedded SQL resource file as a string from the assembly's manifest resources.
    /// </summary>
    /// <param name="path">The resource path, which is used to locate the embedded SQL file.</param>
    /// <returns>The content of the SQL resource file as a string.</returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the specified resource file could not be found in the assembly's manifest resources.
    /// </exception>
    private static string LoadResource(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"LogixDb.Data.SqlServer.{path}.sql";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"SQL resource '{resourceName}' not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
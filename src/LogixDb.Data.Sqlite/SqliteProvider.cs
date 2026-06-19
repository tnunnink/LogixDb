using System.Data.Common;
using System.Reflection;
using Dapper;
using LogixDb.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides functionality to interact with SQLite databases, including creating connections,
/// initializing writers for transactional operations, and preparing SQL scripts.
/// Implements the <see cref="LogixDb.Data.Abstractions.IDbProvider"/> interface.
/// </summary>
public class SqliteProvider : IDbProvider
{
    /// <summary>
    /// Represents the database connection configuration used by the <see cref="SqliteProvider"/>
    /// for establishing and managing connections to a SQLite database. This variable is
    /// initialized with an instance of <see cref="DbConnectionInfo"/> containing the required
    /// database connection details such as the provider type, source, and optional credentials.
    /// </summary>
    private readonly DbConnectionInfo _connection;

    /// <summary>
    /// Provides functionality to interact with SQLite databases, including creating connections,
    /// initializing writers for transactional operations, and preparing SQL scripts.
    /// Implements the <see cref="LogixDb.Data.Abstractions.IDbProvider"/> interface.
    /// </summary>
    public SqliteProvider(DbConnectionInfo connection)
    {
        _connection = connection;
        ConfigureSqlite();
    }

    /// <inheritdoc />
    public async Task<DbConnection> OpenConnection(CancellationToken token = default)
    {
        if (!File.Exists(_connection.Source))
            throw new FileNotFoundException($"Database file not found: {_connection.Source}");

        var sqliteConnection = new SqliteConnection(_connection.ToConnectionString());
        await sqliteConnection.OpenAsync(token);
        return sqliteConnection;
    }

    /// <inheritdoc />
    public IDbWriter GetWriter()
    {
        return new SqliteWriter(this);
    }

    /// <inheritdoc />
    public string GetManagerScript(ScriptName scriptName)
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
        var resourceName = $"LogixDb.Data.Sqlite.{path}.sql";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"SQL resource '{resourceName}' not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Configures SQLite-specific settings for Dapper by modifying type mappings and adding a custom type handler for GUIDs.
    /// Removes existing type maps for <see cref="Guid"/> and nullable <see cref="Guid"/> types
    /// and registers a custom <see cref="SqliteGuidHandler"/> to handle GUID serialization and deserialization.
    /// </summary>
    private static void ConfigureSqlite()
    {
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));
        SqlMapper.AddTypeHandler(new SqliteGuidHandler());
    }
}
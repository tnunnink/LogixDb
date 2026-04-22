using System.Reflection;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides a collection of SQL query scripts for interacting with an SQLite database.
/// The class defines static properties representing various SQL commands
/// by loading them from embedded resource files.
/// </summary>
internal static class SqliteScript
{
    /// <summary>
    /// Represents a SQL script used to insert a new target into the database.
    /// This property contains the SQL command to ensure the creation of a target entry
    /// and associates a new version with the target in the database.
    /// </summary>
    public static string PostTarget => Get(nameof(PostTarget));

    /// <summary>
    /// Represents a SQL script used to restore a target in the database.
    /// This property contains the SQL command that creates a record for a restored version
    /// of a target and associates related metadata, such as the restoration timestamp and user information.
    /// </summary>
    public static string PostInstance => Get(nameof(PostInstance));

    /// <summary>
    /// Represents a SQL script used to insert metadata associated with a target version into the database.
    /// This property contains the SQL command that adds key-value pairs of additional information
    /// about a target version to the appropriate database table.
    /// </summary>
    public static string PostInfo => Get(nameof(PostInfo));

    /// <summary>
    /// Represents a SQL query script used to retrieve a list of targets from the database.
    /// This property contains the SQL command to fetch all targets or filter them
    /// based on specific criteria, such as the target key.
    /// </summary>
    public static string ListTargets => Get(nameof(ListTargets));

    /// <summary>
    /// Represents a SQL script used to retrieve a specific target by its version number
    /// from the SQLite database. This property provides the SQL command necessary
    /// for querying a target based on the provided target key and version for precise data retrieval.
    /// </summary>
    public static string GetTargetByVersion => Get(nameof(GetTargetByVersion));

    /// <summary>
    /// Represents the SQL query script used to retrieve the latest version of a target
    /// associated with a specific key from the database. This query ensures that
    /// the most recent target version is fetched while providing an efficient way
    /// to filter by the target key.
    /// </summary>
    public static string GetTargetByLatest => Get(nameof(GetTargetByLatest));

    /// <summary>
    /// Represents a SQL script used to delete an existing target from the database.
    /// This property provides the SQL command necessary to remove a target entry,
    /// including any related data or associations that must also be cleaned up.
    /// </summary>
    public static string DeleteTarget => Get(nameof(DeleteTarget));

    /// <summary>
    /// Represents a SQL script used to delete all instances of a target from the database.
    /// This property contains the SQL command that removes all version instances
    /// associated with a specific target, effectively clearing its historical data.
    /// </summary>
    public static string DeleteTargetInstances => Get(nameof(DeleteTargetInstances));

    /// <summary>
    /// Represents the SQL script used to delete a specific version instance of a target from the database.
    /// This property contains the command that facilitates the removal of a versioned instance
    /// associated with a given target, based on specified criteria.
    /// </summary>
    public static string DeleteVersionInstance => Get(nameof(DeleteVersionInstance));

    /// <summary>
    /// Represents the SQL script used to delete multiple versions of a target
    /// identified by a specific version number. The command removes all
    /// associated data linked to the specified version from the database.
    /// </summary>
    public static string DeleteVersionsByNumber => Get(nameof(DeleteVersionsByNumber));

    /// <summary>
    /// Represents a SQL script used to delete version entries from the database
    /// with a creation date earlier than a specified date.
    /// This property provides the SQL command for purging older version records
    /// to maintain database cleanliness and manage data retention policies.
    /// </summary>
    public static string DeleteVersionsBeforeDate => Get(nameof(DeleteVersionsBeforeDate));

    /// <summary>
    /// Represents a SQL script used to retrieve the list of component-related database tables.
    /// This property contains the SQL command to query and return a collection of table names
    /// associated with components stored in the database.
    /// </summary>
    public static string GetComponentTables => Get(nameof(GetComponentTables));

    /// <summary>
    /// Retrieves the content of an embedded SQL script resource by its name.
    /// </summary>
    /// <param name="scriptName">The name of the SQL script to retrieve, excluding file extension.</param>
    /// <returns>The content of the requested SQL script as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified SQL script is not found as an embedded resource.</exception>
    private static string Get(string scriptName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        // The resource name usually follows the pattern: {ProjectNamespace}.{Folder}.{FileName}.sql
        var resourceName = $"LogixDb.Data.Sqlite.Scripts.{scriptName}.sql";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"SQL script '{scriptName}' not found as embedded resource.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
namespace LogixDb.Data;

/// <summary>
/// Configuration options for filtering which database tables or component types should be processed
/// during snapshot transformation and database migration operations.
/// Supports both include (whitelist) and exclude (blacklist) patterns for selective processing.
/// </summary>
public class DbOptions
{
    /// <summary>
    /// Gets the list of table names to include for processing.
    /// When this list contains values, only the specified components will be processed (whitelist behavior).
    /// If empty, all components are eligible unless explicitly excluded via <see cref="Exclude"/>.
    /// Matching is case-insensitive.
    /// </summary>
    public string[] Include { get; init; } = [];

    /// <summary>
    /// Gets the list of table names to exclude from processing.
    /// This property is ignored when <see cref="Include"/> contains values (whitelist takes priority).
    /// When <see cref="Include"/> is empty, all components except those listed here will be processed (blacklist behavior).
    /// Matching is case-insensitive.
    /// </summary>
    public string[] Exclude { get; init; } = [];

    /// <summary>
    /// Determines whether the specified table name is configured based on the include and exclude filters in this instance of <see cref="DbOptions"/>.
    /// The method checks if the table name exists in the include list and does not exist in the exclude list.
    /// </summary>
    /// <param name="tableName">The name of the table to check configuration for.</param>
    /// <returns>
    /// Returns <c>true</c> if the table is included and not excluded; otherwise, <c>false</c>.
    /// </returns>
    public bool IsConfigured(string tableName)
    {
        var included = Include.Contains(tableName, StringComparer.CurrentCultureIgnoreCase);
        var excluded = Exclude.Contains(tableName, StringComparer.CurrentCultureIgnoreCase);
        return included && !excluded;
    }
}
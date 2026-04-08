namespace LogixDb.Data;

/// <summary>
/// Configuration options for filtering which database tables or component types should be processed
/// during snapshot transformation and database migration operations.
/// Supports both include (whitelist) and exclude (blacklist) patterns for selective processing.
/// </summary>
public class TableOptions
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
}
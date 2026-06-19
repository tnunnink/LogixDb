namespace LogixDb.Data;

/// <summary>
/// Represents the result of a database migration operation.
/// </summary>
public record MigrationResult
{
    /// <summary>
    /// Gets a value indicating whether the migration operation completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the error message if the migration operation failed; otherwise, <c>null</c>.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets the list of migration scripts that were successfully executed during the operation.
    /// </summary>
    public List<string> Executed { get; init; } = [];
}
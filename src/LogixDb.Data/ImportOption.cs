namespace LogixDb.Data;

/// <summary>
/// Defines strategies for resolving conflicts when importing snapshots into the database.
/// Determines how new snapshot data should be handled when existing snapshots are present for the same target.
/// </summary>
public enum ImportOption
{
    /// <summary>
    /// Adds the new snapshot to the database without removing or replacing any existing snapshots.
    /// All previous snapshots for the target are preserved.
    /// </summary>
    Append,

    /// <summary>
    /// Replaces only the most recent snapshot for the target with the new snapshot.
    /// Older snapshots are preserved, maintaining historical data beyond the latest entry.
    /// </summary>
    ReplaceLatest,

    /// <summary>
    /// Removes all existing snapshots for the target and replaces them with the new snapshot.
    /// This effectively clears the entire snapshot history for the target.
    /// </summary>
    ReplaceAll
}
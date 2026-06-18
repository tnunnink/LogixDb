namespace LogixDb.Data;

/// <summary>
/// Represents the current status of an import session.
/// </summary>
public enum ImportStatus
{
    /// <summary>
    /// The import has been queued, but processing has not yet started.
    /// </summary>
    Pending,

    /// <summary>
    /// The import is currently being processed.
    /// </summary>
    Processing,

    /// <summary>
    /// The import has been successfully processed and completed.
    /// </summary>
    Complete,

    /// <summary>
    /// The import encountered an error during processing and failed to complete.
    /// </summary>
    Failed
}
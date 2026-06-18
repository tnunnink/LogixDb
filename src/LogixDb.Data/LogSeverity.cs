namespace LogixDb.Data;

/// <summary>
/// Represents the severity level of a log entry in an ingestion session.
/// Used to categorize log messages by their importance and type.
/// </summary>
public enum LogSeverity
{
    /// <summary>
    /// Informational message indicating normal operation or status updates.
    /// </summary>
    Info,

    /// <summary>
    /// Warning message indicating a potential issue that does not prevent operation from continuing.
    /// </summary>
    Warning,

    /// <summary>
    /// Error message indicating a failure or critical issue that may affect operation.
    /// </summary>
    Error
}
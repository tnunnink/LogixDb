namespace LogixDb.Data;

/// <summary>
/// Represents a log entry for an import process, capturing key information such as the import identifier,
/// the severity level of the log, and a descriptive message.
/// </summary>
public record ImportLog(
    Guid ImportId,
    LogSeverity LogSeverity,
    string LogMessage,
    string? LogException = null
);
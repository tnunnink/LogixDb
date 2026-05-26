namespace LogixDb.Cli.Common;

/// <summary>
/// Defines standard exit codes returned by the L5Sharp CLI application to indicate
/// the result of command execution.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// Indicates a general unspecified error occurred during command execution.
    /// </summary>
    public const int InternalError = 1;

    /// <summary>
    /// Indicates a system-level error occurred, such as I/O or permissions issues.
    /// </summary>
    public const int SystemError = 2;

    /// <summary>
    /// Indicates incorrect command usage, such as invalid arguments or missing required parameters.
    /// </summary>
    public const int UsageError = 3;

    /// <summary>
    /// Indicates the L5X file format is invalid or malformed.
    /// </summary>
    public const int FormatError = 4;

    /// <summary>
    /// Represents an error code indicating that the specified target, resource, or entity
    /// was not found during the execution of a command.
    /// </summary>
    public const int NotFound = 6;
}
namespace LogixDb.Data;

/// <summary>
/// Represents an import operation for processing files within the system.
/// </summary>
public sealed record Import
{
    /// <summary>
    /// Gets the unique identifier for the import operation. This identifier is
    /// automatically generated as a version 7 GUID and is used to distinctly
    /// track and manage the import instance across various services and tests.
    /// </summary>
    public Guid ImportId { get; } = Guid.CreateVersion7();

    /// <summary>
    /// Gets or initializes the current status of the import operation.
    /// Defaults to <see cref="ImportStatus.Pending"/> when a new import session is created.
    /// </summary>
    public ImportStatus ImportStatus { get; set; } = ImportStatus.Pending;

    /// <summary>
    /// Gets or initializes the type of sender that initiated the import operation.
    /// This property indicates the origin of the request, such as a command-line interface (CLI),
    /// an application programming interface (API), or FactoryTalk AssetCentre (FTAC).
    /// It helps in identifying where the source came from.
    /// </summary>
    public required SourceType SourceType { get; init; }

    /// <summary>
    /// Specifies the type of file being processed or referenced.
    /// Used to distinguish between supported file formats in the system,
    /// such as L5X and ACD, for processing or validation purposes.
    /// </summary>
    public required FileType FileType { get; init; }

    /// <summary>
    /// Represents the name of the file associated with the source.
    /// This property typically includes the file's full path or its designated name,
    /// used for identification and processing in the ingestion workflow.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets or initializes the file system path where the source file is uploaded.
    /// This path represents the location where the file is temporarily stored for
    /// processing and ingestion into the system.
    /// </summary>
    public required string DropPath { get; init; }

    /// <summary>
    /// Gets the fully qualified file path constructed by combining the drop path,
    /// file name, unique import identifier, and file type. This property ensures
    /// the generated file path is uniquely identifiable and correctly formatted
    /// for processing and tracking within the system.
    /// </summary>
    public string FilePath => Path.Combine(DropPath, $"{FileName}.{ImportId:N}.{FileType}");

    /// <summary>
    /// Represents additional data or descriptive key-value pairs associated with the source.
    /// Provides contextual or supplementary details that are not part of the core properties.
    /// </summary>
    public Dictionary<string, string> Metadata { get; } = [];

    /// <summary>
    /// Logs an informational message associated with the current import operation.
    /// Creates an <see cref="ImportLog"/> with a severity level of <see cref="LogSeverity.Info"/>
    /// and includes the provided descriptive message.
    /// </summary>
    /// <param name="message">The descriptive message providing details about the process being logged.</param>
    /// <returns>An <see cref="ImportLog"/> instance containing the import identifier, severity level, and the descriptive message.</returns>
    public ImportLog Info(string message)
    {
        return new ImportLog(ImportId, LogSeverity.Info, message);
    }

    /// <summary>
    /// Logs a warning message associated with the current import operation.
    /// Creates an <see cref="ImportLog"/> with a severity level of <see cref="LogSeverity.Warning"/>
    /// and includes the provided descriptive message.
    /// </summary>
    /// <param name="message">The descriptive message providing details about the warning being logged.</param>
    /// <returns>An <see cref="ImportLog"/> instance containing the import identifier, severity level, and the descriptive message.</returns>
    public ImportLog NewWarning(string message)
    {
        return new ImportLog(ImportId, LogSeverity.Warning, message);
    }

    /// <summary>
    /// Logs an error associated with the current import operation.
    /// The error log includes the severity level set to <see cref="LogSeverity.Error"/>,
    /// a descriptive message about the error, and details of the exception that occurred.
    /// </summary>
    /// <param name="message">The descriptive message providing details about the error.</param>
    /// <param name="exception">The exception instance containing the error details.</param>
    /// <returns>An <see cref="ImportLog"/> instance representing the error log entry,
    /// including the import identifier, severity level, error message, and exception details.</returns>
    public ImportLog Error(string message, Exception exception)
    {
        return new ImportLog(ImportId, LogSeverity.Error, message, exception.ToString());
    }

    /// <summary>
    /// Creates a new <see cref="Import"/> instance with a generated unique identifier,
    /// file type parsed from the file extension, and a computed file path within the specified drop directory.
    /// Optionally populates the metadata dictionary with provided key-value pairs.
    /// </summary>
    /// <param name="fileName">The original name of the file, including its extension, used to determine the file type.</param>
    /// <param name="dropPath">The directory path where the file will be stored, combined with the generated source ID and file type.</param>
    /// <param name="sourceType"></param>
    /// <param name="metadata">An optional collection of key-value pairs to populate the <see cref="Metadata"/> dictionary.</param>
    /// <returns>A new <see cref="Import"/> instance with all required properties initialized.</returns>
    public static Import Create(
        string fileName,
        string dropPath,
        SourceType sourceType,
        IDictionary<string, string>? metadata = null
    )
    {
        var fileType = Enum.Parse<FileType>(Path.GetExtension(fileName).Trim('.'));

        var source = new Import
        {
            SourceType = sourceType,
            FileType = fileType,
            DropPath = dropPath,
            FileName = fileName
        };

        if (metadata is not null)
        {
            foreach (var kvp in metadata)
                source.Metadata[kvp.Key] = kvp.Value;
        }

        return source;
    }
}
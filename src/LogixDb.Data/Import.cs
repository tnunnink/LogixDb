namespace LogixDb.Data;

/// <summary>
/// Represents an import operation for processing files within the system.
/// </summary>
public sealed record Import
{
    private const string Logixdb = "LogixDb";

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
    public ImportStatus Status { get; set; } = ImportStatus.Pending;

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
    public required string FilePath { get; init; }

    /// <summary>
    /// Gets the fully qualified path of the import file, constructed by combining the specified file path
    /// and the file name with its appropriate extension based on the file type. This property is used for
    /// accessing or referencing the file during the import operation.
    /// </summary>
    public string SourceFile => Path.Combine(FilePath, $"{FileName}.{FileType}");

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
    /// Adds a key-value pair to the metadata collection associated with the import operation.
    /// If the key already exists, its value will be updated with the provided value.
    /// </summary>
    /// <param name="key">The key representing the metadata entry. It cannot be null, empty, or consist only of whitespace.</param>
    /// <param name="value">The value corresponding to the metadata key. It cannot be null, empty, or consist only of whitespace.</param>
    /// <exception cref="ArgumentException">Thrown when the key or value is null, empty, or consists only of whitespace.</exception>
    public void AddData(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("The metadata key cannot be null, empty, or consist only of whitespace.",
                nameof(key));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("The metadata value cannot be null, empty, or consist only of whitespace.",
                nameof(value));

        Metadata[key] = value;
    }

    /// <summary>
    /// Adds or updates metadata entries in the current import operation.
    /// If the provided keys already exist in the metadata dictionary, their values will be replaced.
    /// </summary>
    /// <param name="metadata">A dictionary containing metadata keys and their corresponding values to be added or updated. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="metadata"/> parameter is null.</exception>
    public void AddData(IDictionary<string, string> metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        foreach (var kvp in metadata)
            Metadata[kvp.Key] = kvp.Value;
    }

    /// <summary>
    /// Gets the full path to the temporary file created during the import operation.
    /// This path is generated dynamically and is composed of the system's temporary directory,
    /// a predefined subdirectory, and the file name with its associated unique identifier
    /// and file type extension. It is primarily used for intermediate processing within
    /// the import workflow.
    /// </summary>
    public string GetTempFile()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Logixdb);
        Directory.CreateDirectory(tempPath);
        return Path.Combine(tempPath, $"{FileName}.{ImportId:N}.{FileType.L5X}");
    }

    /// <summary>
    /// Deletes the temporary file associated with the current import operation.
    /// Constructs the temporary file path based on the static LogixDb temp directory,
    /// the unique import identifier, and the specified file type.
    /// If the file exists, it is removed from the system.
    /// </summary>
    public void ClearTemp()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Logixdb);
        var filePath = Path.Combine(tempPath, $"{FileName}.{ImportId:N}.{FileType.L5X}");
        File.Delete(filePath);
    }

    /// <summary>
    /// Creates an instance of the <see cref="Import"/> class using the specified source file and source type.
    /// Determines the file type, file name, and directory path from the provided source file.
    /// </summary>
    /// <param name="sourceFile">The full path to the source file to be imported.</param>
    /// <param name="sourceType">The source type associated with the import, indicating the origin of the file (e.g., CLI, API, FTAC).</param>
    /// <returns>An <see cref="Import"/> instance populated with the extracted file attributes and the specified source type.</returns>
    public static Import Create(string sourceFile, SourceType sourceType)
    {
        var fileType = Enum.Parse<FileType>(Path.GetExtension(sourceFile).Trim('.'), ignoreCase: true);
        var fileName = Path.GetFileNameWithoutExtension(sourceFile);
        var filePath = Path.GetDirectoryName(Path.GetFullPath(sourceFile));

        if (filePath is null)
            throw new InvalidOperationException(
                $"Unable to determine the directory path for the source file: {sourceFile}");

        var source = new Import
        {
            SourceType = sourceType,
            FileType = fileType,
            FilePath = filePath,
            FileName = fileName
        };

        return source;
    }
}
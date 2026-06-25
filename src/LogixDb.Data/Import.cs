using L5Sharp.Core;
using LogixConverter.Abstractions;

namespace LogixDb.Data;

/// <summary>
/// Represents an import operation for handling source files, tracking metadata,
/// and managing the lifecycle of the import process within the system.
/// </summary>
public sealed record Import : IDisposable
{
    /// <summary>
    /// Represents the directory used as the dropzone for imported files.
    /// This directory serves as a centralized location where files are placed
    /// for processing within the system.
    /// </summary>
    private static readonly DirectoryInfo ImportDirectory = new(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "LogixDb")
    );

    /// <summary>
    /// Generates the full file path for the source file associated with the current import operation.
    /// Combines the predefined import directory, file name, import identifier, and file type to construct the path.
    /// </summary>
    private readonly string _sourceFile;

    /// <summary>
    /// Represents the path of the file being processed during an import operation.
    /// This file is derived from the import configuration and is stored within a
    /// predefined directory for handling and processing within the system.
    /// </summary>
    private readonly string _importFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="Import"/> class with the specified source type, file type, and file name.
    /// Constructs the source file and import file paths based on the import directory and the provided parameters.
    /// </summary>
    /// <param name="sourceType">The source type indicating the origin of the import operation (e.g., CLI, API, FTAC).</param>
    /// <param name="fileType">The file type specifying the format of the file being imported (e.g., L5X, ACD).</param>
    /// <param name="fileName">The name of the file without its extension.</param>
    private Import(SourceType sourceType, FileType fileType, string fileName)
    {
        SourceType = sourceType;
        FileType = fileType;
        FileName = fileName;

        _sourceFile = Path.Combine(ImportDirectory.FullName, $"{FileName}.{ImportId}.{FileType}");
        _importFile = Path.Combine(ImportDirectory.FullName, $"{FileName}.{ImportId}.{FileType.L5X}");
    }

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
    public SourceType SourceType { get; }

    /// <summary>
    /// Specifies the type of file being processed or referenced.
    /// Used to distinguish between supported file formats in the system,
    /// such as L5X and ACD, for processing or validation purposes.
    /// </summary>
    public FileType FileType { get; }

    /// <summary>
    /// Represents the name of the file associated with the source.
    /// This property typically includes the file's full path or its designated name,
    /// used for identification and processing in the ingestion workflow.
    /// </summary>
    public string FileName { get; }

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
    public ImportLog Warning(string message)
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
    /// Creates an instance of the <see cref="Import"/> class using the specified source file and source type.
    /// Determines the file type, file name, and directory path from the provided source file.
    /// </summary>
    /// <param name="sourceFile">The full path to the source file to be imported.</param>
    /// <param name="sourceType">The source type associated with the import, indicating the origin of the file (e.g., CLI, API, FTAC).</param>
    /// <returns>An <see cref="Import"/> instance populated with the extracted file attributes and the specified source type.</returns>
    public static Import Create(string sourceFile, SourceType sourceType)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        var fileType = Enum.Parse<FileType>(Path.GetExtension(sourceFile).Trim('.'), ignoreCase: true);
        var fileName = Path.GetFileNameWithoutExtension(sourceFile);

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("The source file name is required.", nameof(sourceFile));

        return new Import(sourceType, fileType, fileName);
    }

    /// <summary>
    /// Asynchronously loads and returns an L5X representation of the source file.
    /// If the source file is an ACD file, it is first converted to L5X format using the provided or default converter.
    /// If the source file is already in L5X format, it is loaded directly without conversion.
    /// </summary>
    /// <param name="converter">An optional converter to use for ACD to L5X conversion. If not provided, a default <see cref="ImportConverter"/> will be used.</param>
    /// <param name="token">A cancellation token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="L5X"/> instance representing the imported file content.</returns>
    /// <remarks>
    /// When the source file type is ACD, this method performs a conversion to L5X format before loading.
    /// The conversion creates a temporary L5X file that is then loaded and returned.
    /// For L5X source files, the file is loaded directly without any conversion step.
    /// </remarks>
    public async Task<L5X> LoadAsync(ILogixFileConverter? converter = null, CancellationToken token = default)
    {
        // Only when the source is an ACD do we need to first handle the conversion
        if (FileType == FileType.ACD)
        {
            converter ??= new ImportConverter();
            var options = new ConversionOptions { Overwrite = true };
            var result = await converter.ConvertAsync(_sourceFile, _importFile, options, token);

            if (!result.Success)
                throw new InvalidOperationException(result.Error);
        }

        // This will either be the converted file or if the source was an L5X, the source file itself.
        return await L5X.LoadAsync(_importFile, token);
    }

    /// <summary>
    /// Creates a writable <see cref="FileStream"/> for the source file associated with the current import operation.
    /// The stream is opened in Create mode with Write access and no sharing, allowing for exclusive write operations.
    /// </summary>
    /// <returns>A new <see cref="FileStream"/> instance for writing to the source file.</returns>
    public FileStream OpenWriter()
    {
        ImportDirectory.Create();
        return new FileStream(_sourceFile, FileMode.Create, FileAccess.Write, FileShare.None);
    }

    /// <summary>
    /// Copies the content of the source file to the import's source file location.
    /// Ensures the import directory exists before performing the copy operation.
    /// </summary>
    /// <param name="sourceFile">The path of the source file to be copied.</param>
    /// <param name="token">A cancellation token to observe while copying the file.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    public async Task<FileInfo> WriteAsync(string sourceFile, CancellationToken token = default)
    {
        ImportDirectory.Create();
        await using var reader = File.OpenRead(sourceFile);
        await using var writer = File.Create(_sourceFile);
        await reader.CopyToAsync(writer, token);
        return new FileInfo(_sourceFile);
    }

    /// <summary>
    /// Writes the content of the specified stream to the file associated with the current import operation.
    /// The data from the provided stream is copied into the file, overwriting any existing content if necessary.
    /// </summary>
    /// <param name="sourceStream">The input stream containing the data to be written to the file.</param>
    /// <param name="token">A token used to propagate notification that the operation should be canceled.</param>
    /// <returns>A task representing the asynchronous operation of writing the stream to the file.</returns>
    public async Task<FileInfo> WriteAsync(Stream sourceStream, CancellationToken token = default)
    {
        ImportDirectory.Create();
        await using var writer = File.Create(_sourceFile);
        await sourceStream.CopyToAsync(writer, token);
        return new FileInfo(_sourceFile);
    }

    /// <summary>
    /// Releases resources associated with the current import operation.
    /// Deletes the source file if it exists, ensuring the cleanup of temporary files
    /// generated or used during the import process.
    /// </summary>
    public void Dispose()
    {
        if (File.Exists(_sourceFile)) File.Delete(_sourceFile);
        if (File.Exists(_importFile)) File.Delete(_importFile);
    }
}
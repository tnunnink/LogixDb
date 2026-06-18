using System.Threading.Channels;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Service.Common;
using Microsoft.Extensions.Options;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LogixDb.Service.Workers;

/// <summary>
/// Handles the uploading and queuing of source files for further processing.
/// </summary>
/// <remarks>
/// This class is responsible for managing the transfer of files from an incoming stream
/// to a designated drop path, ensuring that a unique file name is generated to prevent conflicts.
/// It also queues the uploaded files for further processing using a provided channel.
/// </remarks>
/// <param name="channel">
/// The channel used to queue the source information for downstream processing.
/// </param>
/// <param name="options">
/// The configuration options containing the drop path for uploaded files.
/// </param>
/// <param name="logger">
/// The logger instance used to log information, errors, and warnings during the upload process.
/// </param>
public class SourceUploadService(
    Channel<Import> channel,
    IDbManager manager,
    IOptions<LogixConfig> options,
    ILogger<SourceUploadService> logger)
{
    /// <summary>
    /// Asynchronously uploads a file to the server and queues it for processing.
    /// </summary>
    /// <param name="file">The file to upload, represented as an <see cref="IFormFile"/>.</param>
    /// <param name="metadata">A collection of metadata associated with the file. Can be null.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an instance of <see cref="Import"/>
    /// with information about the uploaded file.
    /// </returns>
    public async Task<Import> UploadAsync(IFormFile file, IDictionary<string, string> metadata)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Upload requested for file: {FileName} ({FileSize} bytes)",
                file.FileName, file.Length);

        var import = await CreateImportSession(file, metadata, CancellationToken.None);
        if (import is null) return null; //todo need to figure out what to return for error to API request.

        await manager.LogImport(
            import.Info("Starting file upload with server"),
            CancellationToken.None
        );

        // Upload the file to the local server drop path
        await using (var stream = new FileStream(import.DropPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Queue the source for processing by the background service
        await channel.Writer.WriteAsync(import);

        await manager.LogImport(
            import.Info("Upload complete - File queued for for processing and ingestion"),
            CancellationToken.None
        );

        return import;
    }

    /// <summary>
    /// Creates a new import session for the specified file, saving related metadata and initializing it in the system.
    /// </summary>
    /// <param name="file">The file to be imported, represented as an <see cref="IFormFile"/>.</param>
    /// <param name="metadata">A dictionary containing metadata associated with the import. Can be null or empty.</param>
    /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="Import"/> instance
    /// if the session is successfully created, or null if an error occurred during the process.
    /// </returns>
    private async Task<Import?> CreateImportSession(IFormFile file, IDictionary<string, string> metadata,
        CancellationToken token)
    {
        try
        {
            Directory.CreateDirectory(options.Value.DropPath);
            var import = Import.Create(file.FileName, options.Value.DropPath, SourceType.API, metadata);
            await manager.PutImport(import, CancellationToken.None);

            await manager.LogImport(
                import.Info($"Import starting for file {file.FileName}"),
                token
            );

            return import;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to create import session for {FileName}. Check database settings and services.",
                file.FileName
            );

            return null;
        }
    }
}
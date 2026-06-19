using System.Threading.Channels;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Service.Common;

namespace LogixDb.Service.Workers;

/// <summary>
/// Provides functionality to handle file uploads to the server and queue them for processing.
/// </summary>
public class SourceUploadService(Channel<Import> channel, IDbManager manager, ILogger<SourceUploadService> logger)
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
        var import = await CreateImportSession(file, metadata, CancellationToken.None);
        if (import is null) return null; //todo need to figure out what to return for error to API request.

        // Upload the file to the local server drop path
        await manager.LogImport(import.Info("Starting file upload with server"));
        await using (var stream = new FileStream(import.SourceFile, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        await manager.LogImport(import.Info($"Upload completed successfully for {file.FileName}"));
        await manager.LogImport(import.Info("Queueing file for processing"));
        await channel.Writer.WriteAsync(import);

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
            Directory.CreateDirectory(Paths.Dropzone);
            var sourceFile = Path.Combine(Paths.Dropzone, file.FileName);

            var import = Import.Create(sourceFile, SourceType.API);
            import.AddData(metadata);

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
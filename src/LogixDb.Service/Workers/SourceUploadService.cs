using System.Threading.Channels;
using LogixDb.Data;
using LogixDb.Data.Abstractions;

namespace LogixDb.Service.Workers;

/// <summary>
/// Provides functionality to handle file uploads to the server and queue them for processing.
/// </summary>
public class SourceUploadService(Channel<Import> channel, IDbManager manager)
{
    /// <summary>
    /// Uploads a file to the server, creates an import record, logs the upload process, and queues the import for processing.
    /// </summary>
    /// <param name="file">The file to be uploaded.</param>
    /// <param name="metadata">Additional metadata associated with the import.</param>
    /// <param name="token">Cancellation token to cancel the operation.</param>
    /// <returns>The created import instance representing the uploaded file.</returns>
    public async Task<Import> UploadAsync(IFormFile file, IDictionary<string, string> metadata,
        CancellationToken token = default)
    {
        var import = Import.Create(file.FileName, SourceType.API);
        import.AddData(metadata);

        await manager.CreateImport(import, token);
        await manager.LogImport(import.Info("Starting file upload with server"), token);

        await import.WriteAsync(file.OpenReadStream(), token);

        await manager.LogImport(import.Info("Upload completed successfully"), token);
        await channel.Writer.WriteAsync(import, token);

        return import;
    }
}
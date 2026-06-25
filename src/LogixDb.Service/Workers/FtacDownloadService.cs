using System.Data;
using System.Threading.Channels;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Service.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace LogixDb.Service.Workers;

/// <summary>
/// Provides background processing functionality for downloading assets and synchronizing
/// them with source information. This service reads asset details from a channel, processes
/// them by simulating a download or retrieval operation, and writes the resulting source
/// information back to another channel for downstream operations or storage.
/// </summary>
public class FtacDownloadService(
    Channel<AssetInfo> assets,
    Channel<Import> imports,
    IDbManager manager,
    IOptions<LogixConfig> options,
    ILogger<FtacDownloadService> logger
) : BackgroundService
{
    /// <summary>
    /// Specifies the size of each data chunk, in bytes, for processing operations.
    /// This value is used to determine the maximum amount of data to be read or written
    /// in a single iteration during asset download or other chunked operations.
    /// </summary>
    /// <remarks>
    /// 1MB seems to be a good chunk size as it speeds up the download significantly.
    /// </remarks>
    private const int ChunkSize = 1048576;

    /// <summary>
    /// Executes the background task to process assets asynchronously. It reads assets from
    /// the provided channel, downloads them, and writes the resulting source information to another channel.
    /// </summary>
    /// <param name="token">A cancellation token to observe while waiting for tasks to complete.</param>
    /// <returns>A task that represents the asynchronous execution of the background operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        await foreach (var asset in assets.Reader.ReadAllAsync(token))
        {
            var import = Import.Create(asset.AssetName, SourceType.FTAC);
            import.AddData(nameof(asset.AssetId), asset.AssetId.ToString());
            import.AddData(nameof(asset.VersionId), asset.VersionId.ToString());
            import.AddData(nameof(asset.VersionNumber), asset.VersionNumber.ToString());
            import.AddData(nameof(asset.AssetName), asset.AssetName);

            try
            {
                await manager.CreateImport(import, token);
                var startMessage = $"Import starting for asset {asset.AssetName} - v{asset.VersionNumber}";
                await manager.LogImport(import.Info(startMessage), token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initiate import with database for {FileName}", asset.AssetName);
                continue;
            }

            try
            {
                await manager.LogImport(import.Info("Opening connection to FTAC database"), token);
                var connectionString = options.Value.GetFtacConnectionString();
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(token);

                await manager.LogImport(import.Info("Reading asset info to determine file length"), token);
                var size = await ReadAssetSize(connection, asset, token);

                await manager.LogImport(import.Info("Downloading asset from FTAC database"), token);
                await DownloadAsset(connection, asset, import, size, token);
                await manager.LogImport(import.Info("Download completed successfully"), token);

                await manager.LogImport(import.Info("Queueing asset for for ingestion"), token);
                await imports.Writer.WriteAsync(import, token);
            }
            catch (Exception ex)
            {
                await manager.MarkImport(import.ImportId, ImportStatus.Failed, token);
                await manager.LogImport(import.Error($"Failed to download asset {asset.AssetName}", ex), token);
                logger.LogError(ex, "Error downloading asset {FileName}", asset.AssetName);
            }
        }
    }

    /// <summary>
    /// Reads the size of an asset by executing a stored procedure on the provided database connection.
    /// The procedure retrieves details about the file size and version, ensuring the asset is valid for processing.
    /// </summary>
    private static async Task<long> ReadAssetSize(SqlConnection connection, AssetInfo asset, CancellationToken token)
    {
        await using var command = new SqlCommand("dbo.arch_ReadFileChunkStart", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@AssetId", SqlDbType.UniqueIdentifier).Value = asset.AssetId;
        command.Parameters.Add("@VersionId", SqlDbType.UniqueIdentifier).Value = asset.VersionId;
        command.Parameters.Add("@VersionNumber", SqlDbType.Int).Direction = ParameterDirection.Output;

        // This is what needs to be used to pass the correct length when reading file chunks.
        var fileLength = command.Parameters.Add("@FileLength", SqlDbType.BigInt);
        fileLength.Direction = ParameterDirection.Output;

        // This can just be used to throw when we fail to call the procedure correctly.
        var result = command.Parameters.Add("@Result", SqlDbType.Int);
        result.Direction = ParameterDirection.Output;

        // These parameters are required by the procedure signature even if not used here
        command.Parameters.Add("@ChecksumCRC32", SqlDbType.BigInt).Direction = ParameterDirection.Output;
        command.Parameters.Add("@ChecksumSHA1", SqlDbType.NVarChar, 40).Direction = ParameterDirection.Output;
        command.Parameters.Add("@ChecksumSHA256", SqlDbType.NVarChar, 64).Direction = ParameterDirection.Output;
        command.Parameters.Add("@InUse", SqlDbType.TinyInt).Direction = ParameterDirection.Output;
        command.Parameters.Add("@InUseUncPath", SqlDbType.NVarChar, 1024).Direction = ParameterDirection.Output;

        await command.ExecuteNonQueryAsync(token);

        if ((int)result.Value != 0)
        {
            throw new Exception($"arch_ReadFileChunkStart failed with ResultCode: {result.Value}");
        }


        return (long)fileLength.Value;
    }

    /// <summary>
    /// Downloads an asset from the FTAC database using the provided connection and streams it to a file.
    /// </summary>
    /// <param name="connection">An open SQL connection to the FTAC database.</param>
    /// <param name="import">An object representing the import operation, including file path and metadata.</param>
    /// <param name="asset">Details about the asset to be downloaded, such as asset ID and version.</param>
    /// <param name="length">The total size, in bytes, of the asset to be downloaded.</param>
    /// <param name="token">A cancellation token to observe while performing the download operation.</param>
    /// <returns>A task that represents the asynchronous download operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation encounters an unexpected state during execution.</exception>
    private static async Task DownloadAsset(SqlConnection connection, AssetInfo asset, Import import, long length,
        CancellationToken token
    )
    {
        await using var writer = import.OpenWrite();
        await using var command = new SqlCommand("dbo.arch_ReadFileChunk", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@AssetId", SqlDbType.UniqueIdentifier).Value = asset.AssetId;
        command.Parameters.Add("@VersionId", SqlDbType.UniqueIdentifier).Value = asset.VersionId;
        var offsetParameter = command.Parameters.Add("@Offset", SqlDbType.Int);
        var sizeParameter = command.Parameters.Add("@Size", SqlDbType.Int);

        long offset = 0;
        while (offset < length)
        {
            var size = (int)Math.Min(ChunkSize, length - offset);
            offsetParameter.Value = (int)offset;
            sizeParameter.Value = size;

            await using var reader = await command.ExecuteReaderAsync(token);

            if (await reader.ReadAsync(token))
            {
                var bytes = (byte[])reader.GetValue(0);
                await writer.WriteAsync(bytes, token);
            }
            else
            {
                throw new InvalidOperationException($"No data returned for offset={offset} size={size}");
            }

            offset += size;
        }
    }
}
using System.Threading.Channels;
using CliWrap;
using L5Sharp.Core;
using LogixConverter.Abstractions;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Service.Common;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Service.Workers;

/// <summary>
/// A background service that ingests sources from a channel and processes them using the specified database
/// and file conversion services. This service ensures that ingestion tasks are handled asynchronously, logging
/// progress and errors as needed.
/// </summary>
/// <remarks>
/// The <see cref="SourceIngestionService"/> listens to an unbounded channel for incoming <see cref="Import"/> items
/// to process. Each item represents a file to be ingested, and the service coordinates database updates,
/// file conversion, and logging for each task. This class inherits from <see cref="BackgroundService"/>,
/// ensuring lifecycle management is integrated with the ASP.NET Core service hosting environment.
/// </remarks>
public class SourceIngestionService(
    Channel<Import> channel,
    IDbManager manager,
    ILogixFileConverter converter,
    IOptions<LogixConfig> options,
    ILogger<SourceIngestionService> logger) : BackgroundService
{
    /// <summary>
    /// Executes the asynchronous background processing operation for ingesting sources from a channel reader.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token that can be used to signal the operation should stop.</param>
    /// <returns>A task that represents the lifecycle of the background execution process.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Startup Validation (Pre-flight check): If the database is not created or migrated, shutdown the app.
        // Try to wait for a good connection to the database to avoid server reboot and service restart issues.
        await ValidateDatabase(stoppingToken);

        await foreach (var import in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                // Signal to the database the import is now being processed (converted, parsed, and ingested).
                import.Status = ImportStatus.Processing;
                await manager.PutImport(import, stoppingToken);
                await manager.LogImport(
                    import.Info($"Starting ingestion process for {import.FileName}"),
                    stoppingToken
                );

                // Create a temp L5X file for processing, either converting it from ACD or just copying it, depending on the file type.
                var tempFile = await ConvertOrCopy(import, stoppingToken);

                await manager.LogImport(import.Info("Loading L5X content from disc"), stoppingToken);
                // Load the L5X file, create a target and add it to the database.
                var content = await L5X.LoadAsync(tempFile, stoppingToken);
                await manager.LogImport(import.Info("Creating new target instance for import"), stoppingToken);
                var target = Target.Create(content, import.FileName);

                // Load the metadata configured for the source instance.
                target.Info.Add(nameof(import.ImportId), import.ImportId.ToString());
                foreach (var item in import.Metadata)
                    target.Info.Add(item.Key, item.Value);

                // Import the target to the database.
                await manager.LogImport(
                    import.Info($"Importing target {target.TargetKey} into LogixDb database"),
                    stoppingToken
                );
                await manager.ImportTarget(target, stoppingToken);

                // Import the target to the database.
                await manager.LogImport(
                    import.Info($"Target {target.TargetName} imported new version with id: {target.VersionId}"),
                    stoppingToken
                );

                await manager.LogImport(import.Info("Cleaning up file uploads and temporary copies"), stoppingToken);
                // Clean up temp and upload files after processing completes.
                File.Delete(import.TempFile);
                File.Delete(import.SourceFile);

                // Signal to the database the import process has completed
                await manager.LogImport(
                    import.Info($"Import complete for target {target.TargetKey} @v{target.VersionNumber}"),
                    stoppingToken
                );

                import.Status = ImportStatus.Complete;
                await manager.PutImport(import, stoppingToken);
            }
            catch (Exception ex)
            {
                await manager.LogImport(
                    import.Error($"Failed to process import for '{import.FileName}'. Review exception for details", ex),
                    stoppingToken
                );

                logger.LogError(ex, "Error processing logix file {FileName}", import.FileName);
            }
        }
    }

    /// <summary>
    /// Determines whether the current database connection is invalid by attempting to verify connectivity.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token to observe while performing the operation.</param>
    /// <returns>A task that represents the asynchronous operation containing a boolean value indicating whether the connection is invalid.</returns>
    private async Task ValidateDatabase(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await manager.ListTargets(token: stoppingToken);
                logger.LogInformation("Database connection verified. Ingestion service started and waiting uploads...");
                break;
            }
            catch (Exception)
            {
                logger.LogWarning(
                    "Failed to connect to the LogixDb target. " +
                    "Ensure the connection string is correct and the database is reachable. " +
                    "Retrying in 60 seconds..."
                );

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    /// <summary>
    /// Converts or copies a source file to a temporary .L5X file, depending on its type.
    /// </summary>
    /// <param name="import">The source information containing the file metadata and type.</param>
    /// <param name="token">The cancellation token to observe while performing the operation.</param>
    /// <returns>A task that represents the asynchronous operation containing the path to the temporary .L5X file.</returns>
    private async Task<string> ConvertOrCopy(Import import, CancellationToken token)
    {
        await manager.LogImport(import.Info("Creating temporary drop path for uploaded file"), token);

        var tempLocation = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "LogixDb",
            "Temp"
        );

        Directory.CreateDirectory(tempLocation);
        var tempFile = Path.Combine(tempLocation, $"{import.FileName}.{import.ImportId:N}.L5X");

        switch (import.FileType)
        {
            case FileType.L5X:
                await CopyToTempFile(import, tempFile, token);
                break;
            case FileType.ACD:
                await ConvertToTempFile(import, tempFile, token);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(import), import.FileType, "Unsupported file type.");
        }

        return tempFile;
    }

    /// <summary>
    /// Converts the content of the source file to the L5X format and writes it to the specified temporary file location.
    /// </summary>
    /// <param name="import">The source information containing the file metadata and location.</param>
    /// <param name="tempFile">The path to the temporary file where the converted content will be written.</param>
    /// <param name="token">The cancellation token to observe while performing the operation.</param>
    /// <returns>A task that represents the asynchronous conversion operation.</returns>
    private async Task ConvertToTempFile(Import import, string tempFile, CancellationToken token)
    {
        await manager.LogImport(import.Info($"Converting {import.FileName} to temp L5X for processing"), token);

        // Use the configured ACD converter on the local machine instead of the default file converter.
        if (options.Value.AcdConverter is not null)
        {
            await manager.LogImport(import.Info("Custom ACD converter detected. Calling convert command"), token);

            await Cli.Wrap(options.Value.AcdConverter)
                .WithArguments(args => args
                    .Add("convert")
                    .Add("-i").Add(import.SourceFile)
                    .Add("-o").Add(tempFile)
                    .Add("--force"))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(token);

            return;
        }

        // Fall back the default file converter
        await manager.LogImport(import.Info("Attempting to convert ACD using Logix SDK on local machine"), token);
        await converter.ConvertAsync(import.SourceFile, tempFile, token: token);
    }

    /// <summary>
    /// Copies the content of the source file to a temporary file at the specified path.
    /// </summary>
    /// <param name="import">The source information containing the file metadata and location.</param>
    /// <param name="tempFile">The path to the temporary file where the content will be copied.</param>
    /// <param name="token">The cancellation token to observe while performing the operation.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    private async Task CopyToTempFile(Import import, string tempFile, CancellationToken token)
    {
        await manager.LogImport(import.Info($"Copying {import.FileName} to temp file for processing"), token);
        await using var reader = File.OpenRead(import.SourceFile);
        await using var writer = File.Create(tempFile);
        await reader.CopyToAsync(writer, token);
    }
}
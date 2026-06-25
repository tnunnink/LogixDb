using System.Threading.Channels;
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
    /// <param name="token">A cancellation token that can be used to signal the operation should stop.</param>
    /// <returns>A task that represents the lifecycle of the background execution process.</returns>
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        // Startup Validation (Pre-flight check): If the database is not created or migrated, shutdown the app.
        // Try to wait for a good connection to the database to avoid server reboot and service restart issues.
        await ValidateDatabase(token);

        await foreach (var import in channel.Reader.ReadAllAsync(token))
        {
            try
            {
                // Signal to the database the import is now being processed (converted, parsed, and ingested).
                import.Status = ImportStatus.Processing;
                await manager.CreateImport(import, token);
                await manager.LogImport(
                    import.Info($"Starting ingestion process for {import.FileName}"),
                    token
                );

                await manager.LogImport(import.Info("Loading L5X content from disc"), token);
                var content = await import.LoadAsync(converter, token);

                await manager.LogImport(import.Info("Creating new target instance for import"), token);
                var target = Target.Create(content, import.FileName);

                // Load the metadata configured for the source instance.
                target.Info.Add(nameof(import.ImportId), import.ImportId.ToString());
                foreach (var item in import.Metadata)
                    target.Info.Add(item.Key, item.Value);

                // Import the target to the database.
                await manager.LogImport(
                    import.Info($"Importing target {target.TargetKey} into LogixDb database"),
                    token
                );
                await manager.ImportTarget(target, token);

                // Import the target to the database.
                await manager.LogImport(
                    import.Info($"Target {target.TargetName} imported new version with id: {target.VersionId}"),
                    token
                );

                await manager.LogImport(import.Info("Cleaning up file uploads and temporary copies"), token);
                // Clean up temp and upload files after processing completes.
                import.Dispose();

                // Signal to the database the import process has completed
                await manager.LogImport(
                    import.Info($"Import complete for target {target.TargetKey} @v{target.VersionNumber}"),
                    token
                );

                import.Status = ImportStatus.Complete;
                await manager.CreateImport(import, token);
            }
            catch (Exception ex)
            {
                await manager.LogImport(
                    import.Error($"Failed to process import for '{import.FileName}'. Review exception for details", ex),
                    token
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
}
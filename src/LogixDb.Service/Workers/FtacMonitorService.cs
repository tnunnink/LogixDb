using System.Threading.Channels;
using LogixDb.Service.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace LogixDb.Service.Workers;

/// <summary>
/// Monitors the FactoryTalk AssetCentre database for new asset versions using native SqlDependency.
/// </summary>
public class FtacMonitorService(
    Channel<AssetInfo> assets,
    IOptions<LogixConfig> options,
    ILogger<FtacMonitorService> logger
) : BackgroundService
{
    /// <summary>
    /// Tracks the timestamp of the most recent check for new asset versions in the
    /// FactoryTalk AssetCentre database. This value is initialized to the current UTC time
    /// and is updated with each successful polling operation to ensure no updates are missed.
    /// </summary>
    private DateTime _lastCheckTime = DateTime.UtcNow;

    /// <summary>
    /// Executes the background task that monitors the FactoryTalk AssetCentre database for new asset versions
    /// and periodically polls for updates until the operation is canceled.
    /// </summary>
    /// <param name="token">A cancellation token used to signal the asynchronous execution to stop.</param>
    /// <returns>A task that represents the background execution operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("FTAC Monitor Service starting as user: {User}", Environment.UserName);

        while (!token.IsCancellationRequested)
        {
            try
            {
                await PollForNewAssets(token);
            }
            catch (SqlException ex) when (ex.Number == 18456)
            {
                if (logger.IsEnabled(LogLevel.Critical))
                    logger.LogCritical(
                        "Access Denied to FTAC Database. Please ensure user '{User}' has SELECT and EXECUTE permissions for the configured database.",
                        Environment.UserName);
                break; //stop trying until reset.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to query FTAC database for changes.");
            }

            // Wait 10 seconds before polling again.
            await Task.Delay(TimeSpan.FromSeconds(10), token);
        }
    }

    /// <summary>
    /// Polls the FactoryTalk AssetCentre database for new asset versions that have been added or updated
    /// since the last check and writes the retrieved asset information to the specified channel.
    /// </summary>
    /// <param name="token">A cancellation token used to signal the async task to stop polling operations.</param>
    /// <returns>A task that represents the asynchronous operation of polling for new assets.</returns>
    private async Task PollForNewAssets(CancellationToken token)
    {
        const string query =
            """
            SELECT [AssetId], [VersionId], [AssetName], [VersionNumber], [AddEditDate]
            FROM [dbo].[arch_AssetVersions] 
            WHERE [AddEditDate] > @LastDate AND [AssetName] LIKE '%.ACD'
            ORDER BY [AddEditDate] ASC
            """;

        var connectionString = options.Value.GetFtacConnectionString();
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@LastDate", _lastCheckTime);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        while (await reader.ReadAsync(token))
        {
            var asset = new AssetInfo
            {
                AssetId = reader.GetGuid(0),
                VersionId = reader.GetGuid(1),
                AssetName = reader.GetString(2),
                VersionNumber = reader.GetInt32(3)
            };

            _lastCheckTime = reader.GetDateTime(4);
            assets.Writer.TryWrite(asset);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Polled New Asset: {AssetName}", asset.AssetName);
        }
    }
}
using System.Text.RegularExpressions;
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
            ORDER BY [AddEditDate]
            """;

        var connectionString = options.Value.GetFtacConnectionString();
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@LastDate", _lastCheckTime);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        while (await reader.ReadAsync(token))
        {
            var assetName = reader.GetString(2);

            if (!IsMatch(assetName, options.Value.FtacFilters))
                continue;

            var asset = new AssetInfo
            {
                AssetId = reader.GetGuid(0),
                VersionId = reader.GetGuid(1),
                AssetName = assetName,
                VersionNumber = reader.GetInt32(3)
            };

            _lastCheckTime = reader.GetDateTime(4);
            assets.Writer.TryWrite(asset);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Polled New Asset: {AssetName}", asset.AssetName);
        }
    }

    /// <summary>
    /// Determines whether the specified asset name matches the given filter criteria.
    /// </summary>
    /// <param name="assetName">The name of the asset to evaluate against the filter criteria.</param>
    /// <param name="filters">
    /// A collection of filter patterns. Filters can include whitelist patterns (e.g., "*.ACD") or blacklist patterns
    /// prefixed with '!' to exclude (e.g., "!*.TMP").
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the asset name satisfies the filter criteria. Returns true if the asset name matches
    /// the whitelist criteria or no filters are defined, and false if it matches any blacklist pattern or does not match the
    /// whitelist patterns.
    /// </returns>
    private static bool IsMatch(string assetName, string[] filters)
    {
        if (filters.Length == 0) return true;
        var whitelists = filters.Where(f => !f.StartsWith('!')).ToList();
        var blacklists = filters.Where(f => f.StartsWith('!')).Select(f => f[1..]).ToList(); //strip out '!'

        // 1. Check Blacklists (Any match = exclude)
        if (blacklists.Any(pattern => FitsMask(assetName, pattern)))
            return false;

        // 2. Check Whitelists (If any exist, must match at least one)
        if (whitelists.Count > 0)
            return whitelists.Any(pattern => FitsMask(assetName, pattern));

        return true;
    }

    /// <summary>
    /// Determines whether the specified file name matches the given file mask pattern.
    /// </summary>
    /// <param name="fileName">The name of the file to be checked against the mask.</param>
    /// <param name="fileMask">The pattern defining the file mask, which may include wildcard characters such as '*' and '?'.</param>
    /// <returns>True if the file name matches the mask; otherwise, false.</returns>
    private static bool FitsMask(string fileName, string fileMask)
    {
        var pattern = "^" + Regex.Escape(fileMask).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
        return Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase);
    }
}
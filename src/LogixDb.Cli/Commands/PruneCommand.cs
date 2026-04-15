using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command for pruning snapshots and other related resources in the LogixDb CLI.
/// </summary>
/// <remarks>
/// The <see cref="PruneCommand"/> class provides functionality to delete specific snapshots,
/// snapshots imported before a specific date, or the latest snapshot tied to a target.
/// Use the provided command options to specify pruning behavior.
/// This command inherits from <see cref="DbCommand"/>, allowing database connection configuration.
/// </remarks>
/// <example>
/// This command supports the following options:
/// - Target: Specifies the target resource to be pruned.
/// - Version: Deletes a snapshot with a specific version number.
/// - SnapshotId: Deletes a snapshot with a specific ID.
/// - Before: Deletes snapshots imported before a given date.
/// - Latest: Deletes the latest snapshot for a specified target.
/// </example>
[PublicAPI]
[Command("prune", Description = "Delete snapshots by version, date, or target")]
public partial class PruneCommand : DbCommand
{
    [CommandOption("target", 't', Description = "Target key to prune (default format: Controller://project_name)")]
    public string? Target { get; set; }

    [CommandOption("version", 'v', Description = "Delete a snapshot with the specified version.")]
    public int Version { get; set; }

    [CommandOption("before", Description = "Delete snapshots imported before the specified date.")]
    public string? Before { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, ILogixDb database, CancellationToken token)
    {
        if (Version > 0) await DeleteByVersion(console, database, token);
        if (!string.IsNullOrWhiteSpace(Before)) await DeleteByDate(console, database, token);
        //todo no version or date, but if target is selected we should just delete that.
    }

    /// <summary>
    /// Deletes a snapshot with the specified version from the database.
    /// </summary>
    /// <param name="console">The console instance used to output status messages.</param>
    /// <param name="database">The database instance used to interact with stored snapshots.</param>
    /// <param name="token">The cancellation token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="CommandException">Thrown when an error occurs during the deletion of the snapshot.</exception>
    private async ValueTask DeleteByVersion(IConsole console, ILogixDb database, CancellationToken token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Target))
                throw new CommandException("--version requires --target to be specified", ErrorCodes.UsageError);

            await database.DeleteSnapshot(Target, Version, token);
            await console.Output.WriteLineAsync($"Deleted snapshot version {Version} for {Target}");
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Prune failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }

    /// <summary>
    /// Deletes snapshots imported before a specified date.
    /// </summary>
    /// <param name="console">The console interface used for output messages.</param>
    /// <param name="database">The database interface that provides functionality for managing snapshots.</param>
    /// /// <param name="token">The cancellation token used to cancel the asynchronous operation.</param>
    /// <exception cref="CommandException">
    /// Thrown when an invalid date format is provided or when a deletion operation fails due to an internal error.
    /// </exception>
    private async ValueTask DeleteByDate(IConsole console, ILogixDb database, CancellationToken token)
    {
        try
        {
            if (!DateTime.TryParse(Before, out var beforeDate))
                throw new CommandException($"Invalid date format: {Before}", ErrorCodes.UsageError);

            await database.DeleteSnapshotsBefore(beforeDate, Target, token);
            await console.Output.WriteLineAsync(
                $"Deleted snapshots before {beforeDate:yyyy-MM-dd}" + (Target != null ? $" for {Target}" : "")
            );
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Prune failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }

    /// <summary>
    /// Deletes all snapshots associated with the specified target key.
    /// </summary>
    /// <param name="console">The console used to write output messages.</param>
    /// <param name="database">The database interface for executing the delete operation.</param>
    /// <param name="token">The cancellation token used to observe cancellation requests.</param>
    /// <exception cref="CommandException">
    /// Thrown when the target key is not specified or an error occurs during the deletion process.
    /// </exception>
    private async ValueTask DeleteTarget(IConsole console, ILogixDb database, CancellationToken token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Target))
                throw new CommandException("--target must be specified", ErrorCodes.UsageError);

            await database.DeleteSnapshots(Target, token);
            await console.Output.WriteLineAsync($"Deleted all snapshots for {Target}");
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Prune failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }
}
using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Core.Abstractions;

namespace LogixDb.Cli.Commands;

/// <summary>
/// 
/// </summary>
[PublicAPI]
[Command("prune", Description = "")]
public class PruneCommand : DbCommand
{
    [CommandOption("target", 't', Description = "Target key to purge (format: targettype://targetname)")]
    public string? Target { get; init; }

    [CommandOption("id", Description = "Specific snapshot ID to delete")]
    public int SnapshotId { get; init; }

    [CommandOption("latest", Description = "Delete the latest snapshot for the specified target")]
    public bool Latest { get; init; }
    
    [CommandOption("before", Description = "Delete snapshots imported before the specified date (format: yyyy-MM-dd)")]
    public string? Before { get; init; }

    /// <inheritdoc />
    protected override ValueTask ExecuteAsync(IConsole console, ILogixDb database)
    {
        throw new NotImplementedException();
    }
}
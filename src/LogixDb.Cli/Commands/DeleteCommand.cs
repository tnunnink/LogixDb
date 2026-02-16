using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Core.Abstractions;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command to purge snapshots from the database.
/// Can purge all snapshots or snapshots for a specific target.
/// </summary>
[PublicAPI]
[Command("delete", Description = "")]
public class DeleteCommand : DbCommand
{
    [CommandOption("target", 't', Description = "Target key to purge (format: targettype://targetname)")]
    public string? Target { get; init; }

    [CommandOption("force", 'f', Description = "Skip confirmation prompt")]
    public bool Force { get; init; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, ILogixDb database)
    {
        throw new NotImplementedException();
    }
}
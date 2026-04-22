using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command to archive a snapshot instance, removing its expanded relational data
/// while preserving the historical snapshot metadata and L5X blob.
/// </summary>
[PublicAPI]
[Command("archive",
    Description = "Removes expanded relational data for a snapshot while preserving its archive record")]
public partial class ArchiveCommand : DbCommand
{
    [CommandOption("target", 't',
        Description = "Target key of the snapshot to archive (format: targettype://targetname)")]
    public string? Target { get; set; }

    [CommandOption("version", 'v',
        Description = "Version number of the snapshot to archive. If 0, the latest version is archived.")]
    public int Version { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Target))
            throw new CommandException("Target key must be specified. Use --target option.", ErrorCodes.UsageError);

        try
        {
            await console.Ansi().Status().StartAsync(
                $"Archiving snapshot '{Target}' version {(Version == 0 ? "latest" : Version)}...",
                _ => manager.ArchiveTarget(Target, Version, token)
            );

            console.Ansi().MarkupLine(
                $"[green]✓[/] Snapshot '{Target}' version {(Version == 0 ? "latest" : Version)} archived successfully");
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Archive failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }
}
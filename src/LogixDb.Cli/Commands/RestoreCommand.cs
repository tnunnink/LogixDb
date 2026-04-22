using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command to restore a snapshot's compressed blob into relational tables.
/// </summary>
[PublicAPI]
[Command("restore", Description = "Expands an archived snapshot into relational tables")]
public partial class RestoreCommand : DbCommand
{
    [CommandOption("target", 't',
        Description = "Target key of the snapshot to restore (format: targettype://targetname)")]
    public string? Target { get; set; }

    [CommandOption("version", 'v',
        Description = "Version number of the snapshot to restore. If 0, the latest version is restored.")]
    public int Version { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Target))
            throw new CommandException("Target key must be specified. Use --target option.", ErrorCodes.UsageError);

        try
        {
            await console.Ansi().Status().StartAsync(
                $"Restoring snapshot '{Target}' version {(Version == 0 ? "latest" : Version)}...",
                _ => manager.RestoreTarget(Target, Version, token)
            );

            console.Ansi().MarkupLine(
                $"[green]✓[/] Snapshot '{Target}' version {(Version == 0 ? "latest" : Version)} restored successfully");
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Restore failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }
}
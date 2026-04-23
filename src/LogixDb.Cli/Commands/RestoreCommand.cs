using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("restore", Description = "Restores target version into relational tables (creates new instance data)")]
public partial class RestoreCommand : DbCommand
{
    [Required]
    [CommandOption("target", 't', Description = "Target key to restore (format: targettype://targetname)")]
    public string? Target { get; set; }

    [CommandOption("version", 'v', Description = "Version number to restore. If 0, the latest version is restored.")]
    public int Version { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Target))
            throw new CommandException("Target key must be specified. Use --target option.", ErrorCodes.UsageError);

        try
        {
            await console.Ansi().Status().StartAsync(
                $"Restoring target '{Target}' version {(Version == 0 ? "latest" : Version)}...",
                _ => manager.RestoreTarget(Target, Version, token)
            );

            console.Ansi().MarkupLine(
                $"[green]✓[/] Target '{Target}' version {(Version == 0 ? "latest" : Version)} restored successfully");
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
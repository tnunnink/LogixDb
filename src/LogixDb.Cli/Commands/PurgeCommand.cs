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
[Command("purge", Description = "Permanently removes a target and its entire version history")]
public partial class PurgeCommand : DbCommand
{
    [Required]
    [CommandOption("target", 't', Description = "Target key to purge (format: targettype://targetname)")]
    public string Target { get; set; } = string.Empty;

    [CommandOption("force", 'f', Description = "Skip confirmation prompt.")]
    public bool Force { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        var confirm = $"Are you sure you want to purge all data for target '{Target}'? This action cannot be undone.";
        if (!Force && !await console.Ansi().ConfirmAsync(confirm, false, token))
        {
            console.Ansi().MarkupLine("[yellow]Operation cancelled[/]");
            return;
        }

        try
        {
            await console.Ansi().Status().StartAsync($"Purging target '{Target}'...",
                _ => manager.DeleteTarget(Target, token)
            );

            console.Ansi().MarkupLine($"[green]✓[/] Target '{Target}' purged successfully");
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Target purge failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }
}
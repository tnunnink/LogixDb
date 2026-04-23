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
[Command("prune", Description = "Deletes all instance data for a specified target (retains version history)")]
public partial class PruneCommand : DbCommand
{
    [Required]
    [CommandOption("target", 't', Description = "Target key to prune (format: targettype://targetname)")]
    public string Target { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Target))
            throw new CommandException("Target key must be specified. Use --target option.", ErrorCodes.UsageError);

        try
        {
            await console.Ansi().Status().StartAsync($"Pruning instances for '{Target}'...",
                _ => manager.PruneTarget(Target, token)
            );

            console.Ansi().MarkupLine($"[green]✓[/] Target instances for '{Target}' pruned successfully");
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
using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("prune", Description = "Removes a specific version of a target while preserving other versions")]
public partial class PruneCommand : DbCommand
{
    [Required]
    [CommandOption("target", 't', Description = "Target key to prune")]
    public string Target { get; set; } = string.Empty;

    [Required]
    [CommandOption("version", 'v', Description = "Version number to prune")]
    public int Version { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Target))
            throw new CommandException(
                "Target key must be specified. Use --target option.",
                ErrorCodes.UsageError
            );

        if (Version <= 0)
            throw new CommandException(
                "Version number must be greater than 0. Use --version option.",
                ErrorCodes.UsageError
            );

        try
        {
            var manager = GetManager();
            
            await console.Ansi().Status().StartAsync($"Pruning instances for '{Target}'...",
                _ => manager.DeleteVersion(Target, Version, token)
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
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command for pruning targets and other related resources in the LogixDb CLI.
/// </summary>
/// <remarks>
/// The <see cref="PruneCommand"/> class provides functionality to delete specific targets,
/// targets imported before a specific date, or the latest instance tied to a target.
/// Use the provided command options to specify pruning behavior.
/// This command inherits from <see cref="DbCommand"/>, allowing database connection configuration.
/// </remarks>
/// <example>
/// This command supports the following options:
/// - Target: Specifies the target resource to be pruned.
/// - Version: Deletes a target with a specific version number.
/// - TargetId: Deletes a target with a specific ID.
/// - Before: Deletes targets imported before a given date.
/// - Latest: Deletes the latest instance for a specified target.
/// </example>
[PublicAPI]
[Command("prune", Description = "Delete targets by version, date, or target")]
public partial class PruneCommand : DbCommand
{
    [CommandOption("target", 't',
        Description = "Target key of the instances to prune (format: targettype://targetname)")]
    public string? Target { get; set; }

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
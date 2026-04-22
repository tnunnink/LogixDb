using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command for listing all targets in the database, optionally filtered by a target key.
/// </summary>
/// <remarks>
/// This command retrieves and displays targets from the database. The results can be filtered
/// by specifying a target key, which is expected in the format "targettype://targetname".
/// The output includes a table of target details such as ID, target key, target type,
/// target name, revision, user, machine, and import/export dates.
/// </remarks>
/// <example>
/// Use this command to query and review targets stored in the database.
/// The filtering by target key can help narrow results for specific targets.
/// </example>
[PublicAPI]
[Command("list", Description = "Lists all target versions, optionally filtered by target key")]
public partial class ListCommand : DbCommand
{
    [CommandOption("target", 't', Description = "Optional target key filter (format: targettype://targetname)")]
    public string? TargetKey { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        try
        {
            var targets = await console.Ansi().Status().StartAsync("Retrieving targets...",
                _ => manager.ListTargets(TargetKey, token)
            );

            OutputResults(console, targets.ToList());
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"List targets failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }

    /// <summary>
    /// Displays a formatted table of targets to the console.
    /// </summary>
    /// <param name="console">The console instance used for rendering output.</param>
    /// <param name="targets">The list of targets to display.</param>
    private static void OutputResults(IConsole console, List<Target> targets)
    {
        if (targets.Count == 0)
        {
            console.Ansi().MarkupLine("[yellow]No targets found[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("Key")
            .AddColumn("Version")
            .AddColumn("Type")
            .AddColumn("Name")
            .AddColumn("Revision")
            .AddColumn("Date")
            .AddColumn("User")
            .AddColumn("Machine")
            .AddColumn("Hash");

        foreach (var target in targets.OrderByDescending(s => s.ImportDate))
        {
            table.AddRow(
                target.TargetKey,
                target.TargetType,
                target.TargetName,
                target.VersionNumber.ToString(),
                target.SoftwareRevision ?? "N/A",
                target.ImportDate.ToString("yyyy-MM-dd HH:mm:ss"),
                target.ImportUser,
                target.ImportMachine,
                target.SourceHash
            );
        }

        console.Ansi().Write(table);
        console.Ansi().MarkupLine($"\n[green]Total:[/] {targets.Count} target(s)");
    }
}
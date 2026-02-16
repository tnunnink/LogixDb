using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Core.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command for listing all snapshots in the database, optionally filtered by a target key.
/// </summary>
/// <remarks>
/// This command retrieves and displays snapshots from the database. The results can be filtered
/// by specifying a target key, which is expected in the format "targettype://targetname".
/// The output includes a table of snapshot details such as ID, target key, target type,
/// target name, revision, user, machine, and import/export dates.
/// </remarks>
/// <example>
/// Use this command to query and review snapshots stored in the database.
/// The filtering by target key can help narrow results for specific targets.
/// </example>
[PublicAPI]
[Command("list", Description = "Lists all snapshots, optionally filtered by target key")]
public class ListCommand : DbCommand
{
    [CommandOption("target", 't', Description = "Optional target key filter (format: targettype://targetname)")]
    public string? TargetKey { get; init; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, ILogixDb database)
    {
        var snapshots = await console.Ansi()
            .Status()
            .StartAsync("Retrieving snapshots...", async _ => (await database.ListSnapshots(TargetKey)).ToList());

        if (snapshots.Count == 0)
        {
            console.Ansi().MarkupLine("[yellow]No snapshots found[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("Id")
            .AddColumn("Target Key")
            .AddColumn("Target Type")
            .AddColumn("Target Name")
            .AddColumn("Imported")
            .AddColumn("Exported")
            .AddColumn("Revision")
            .AddColumn("User")
            .AddColumn("Machine");

        foreach (var snapshot in snapshots.OrderByDescending(s => s.ImportDate))
        {
            table.AddRow(
                snapshot.SnapshotId.ToString(),
                snapshot.TargetKey,
                snapshot.TargetType,
                snapshot.TargetName,
                snapshot.ImportDate.ToString("yyyy-MM-dd HH:mm:ss"),
                snapshot.ExportDate.ToString("yyyy-MM-dd HH:mm:ss"),
                snapshot.SoftwareRevision ?? "N/A",
                snapshot.ImportUser,
                snapshot.ImportMachine
            );
        }

        console.Ansi().Write(table);
        console.Ansi().MarkupLine($"\n[green]Total:[/] {snapshots.Count} snapshot(s)");
    }
}
using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Core.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command to list all snapshots in the database, optionally filtered by target key.
/// </summary>
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

        var table = new Table()
            .Border(TableBorder.Rounded)
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
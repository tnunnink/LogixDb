using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("list", Description = "Lists all target versions, optionally filtered by target key")]
public partial class ListCommand : DbCommand
{
    [CommandOption("target", 't', Description = "Optional target key filter")]
    public string? TargetKey { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, CancellationToken token)
    {
        try
        {
            var manager = GetManager();

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
            .AddColumn("Revision")
            .AddColumn("Date")
            .AddColumn("User")
            .AddColumn("Machine");

        foreach (var target in targets.OrderByDescending(s => s.ImportDate))
        {
            table.AddRow(
                target.TargetKey,
                target.VersionNumber.ToString(),
                target.TargetType,
                target.SoftwareRevision ?? "N/A",
                target.ImportDate.ToString("yyyy-MM-dd HH:mm:ss"),
                target.ImportUser,
                target.ImportMachine
            );
        }

        console.Ansi().Write(table);
        console.Ansi().MarkupLine($"\n[green]Total:[/] {targets.Count} target(s)");
    }
}
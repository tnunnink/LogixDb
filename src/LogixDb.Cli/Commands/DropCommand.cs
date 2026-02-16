using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Core.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command-line interface (CLI) command that drops the entire database.
/// This command permanently deletes all tables and data within the database.
/// </summary>
/// <remarks>
/// Users are prompted for confirmation before the operation is executed to prevent
/// unintended data loss. The operation is irreversible once initiated.
/// </remarks>
[PublicAPI]
[Command("drop", Description = "Drops the entire database, permanently deleting all tables and data")]
public class DropCommand : DbCommand
{
    private const string Confirm =
        "Are you sure you want to drop the entire database? This action cannot be undone.";

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, ILogixDb database)
    {
        if (!await console.Ansi().ConfirmAsync(Confirm))
        {
            console.Ansi().MarkupLine("[yellow]Operation cancelled[/]");
            return;
        }

        await console.Ansi()
            .Status()
            .StartAsync("Dropping database...", _ => database.Drop());

        console.Ansi().MarkupLine("[green]✓[/] Database dropped successfully");
    }
}
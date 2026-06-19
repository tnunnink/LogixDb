using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("migrate", Description = "Runs migrations to create and/or ensure the latest database schema")]
public partial class MigrateCommand : DbCommand
{
    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, CancellationToken token)
    {
        try
        {
            var connection = ParseConnection();
            var migrator = GetMigrator(connection);

            var result = await console.Ansi().Status().StartAsync("Migrating database...",
                _ => migrator.Migrate(connection, token)
            );

            if (!result.Success)
            {
                console.Ansi().MarkupLine($"[red]Error:[/] {result.Error}");
                return;
            }

            if (result.Executed.Count > 0)
            {
                console.Ansi().MarkupLine("[green]✓[/] Database updated successfully");
                foreach (var script in result.Executed)
                    console.Ansi().MarkupLine($"  [blue]-[/] Applied: {script}");
            }
            else
            {
                console.Ansi().MarkupLine("[yellow]![/] Database is already up to date.");
            }
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Database migration failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }
}
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("migrate", Description = "Runs migrations to create and/or ensure the latest database schema")]
public partial class MigrateCommand : DbCommand
{
    [CommandOption("components", Description = "Specifies the Logix components to include during migration")]
    public ComponentOptions Components { get; set; } = ComponentOptions.All;

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        try
        {
            await console.Ansi().Status().StartAsync("Migrating database...",
                _ => manager.Migrate(Components, token)
            );

            console.Ansi().MarkupLine("[green]✓[/] Database migration completed successfully");
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
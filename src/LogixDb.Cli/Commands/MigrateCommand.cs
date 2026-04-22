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
/// Represents a CLI command to execute database migrations. This command ensures that the database schema
/// is up to date with the latest version, creating the schema if it does not already exist.
/// </summary>
/// <remarks>
/// This command is implemented as part of CLI tools for managing and maintaining a database.
/// It handles exceptions during migration and provides relevant feedback to the console regarding
/// the status of the migration process.
/// </remarks>
/// <example>
/// The command can be executed with specific database options such as connection string, provider type,
/// authentication credentials, and other optional parameters inherited from the <c>DbCommand</c> class.
/// </example>
[PublicAPI]
[Command("migrate", Description = "Runs migrations to create and/or ensure the latest database schema")]
public partial class MigrateCommand : DbCommand
{
    /// <summary>
    /// Gets or initializes the collection of Logix components to include during the migration process.
    /// Multiple components can be specified (e.g., --components Controller Tag).
    /// If not specified, all components are included by default.
    /// </summary>
    [CommandOption("components", Description = "Specifies the Logix components to include during migration")]
    public ComponentOptions Components { get; set; } = ComponentOptions.All;

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        
        try
        {
            await console.Ansi()
                .Status()
                .StartAsync("Migrating database...", _ => manager.Migrate(Components, token));

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
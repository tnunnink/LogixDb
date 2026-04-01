using CliFx.Attributes;
using CliFx.Exceptions;
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
public class MigrateCommand : DbCommand
{
    /// <summary>
    /// Gets or initializes the collection of table names to include during the migration process.
    /// If specified, only tables matching these names will be migrated. If empty, all tables are included by default.
    /// </summary>
    [CommandOption("include", Description = "Specifies the table names to include during migration")]
    public string[] Include { get; init; } = [];

    /// <summary>
    /// Gets or initializes the collection of table names to exclude during the migration process.
    /// Tables matching these names will be skipped during migration.
    /// </summary>
    [CommandOption("exclude", Description = "Specifies the table names to exclude during migration")]
    public string[] Exclude { get; init; } = [];

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, ILogixDb database, CancellationToken token)
    {
        var options = new DbOptions { Include = Include, Exclude = Exclude };

        try
        {
            await console.Ansi()
                .Status()
                .StartAsync("Migrating database...", _ => database.Migrate(options, token));

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
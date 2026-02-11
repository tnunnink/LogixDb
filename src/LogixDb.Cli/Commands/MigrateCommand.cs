using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Core.Abstractions;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command to perform database migration operations using the LogixDB system.
/// This command facilitates the migration of database schemas or data structures
/// to ensure compatibility with updated application requirements.
/// </summary>
/// <remarks>
/// The command interacts with an instance of <see cref="ILogixDatabase"/> provided
/// by the <see cref="ILogixDatabaseFactory"/> to execute database migration tasks.
/// </remarks>
/// <example>
/// This command is typically invoked via CLI to manage database migrations.
/// Users can specify parameters such as database provider, authentication type,
/// and other related configurations as needed.
/// </example>
[PublicAPI]
[Command("migrate", Description = "")]
public class MigrateCommand(ILogixDatabaseFactory factory) : DbCommand(factory)
{
    /// <inheritdoc />
    protected override ValueTask ExecuteAsync(IConsole console, ILogixDatabase database)
    {
        database.Migrate();

        //todo write something to the console?

        return ValueTask.CompletedTask;
    }
}
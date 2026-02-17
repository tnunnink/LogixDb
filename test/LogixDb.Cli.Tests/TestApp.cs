using CliFx;
using CliFx.Infrastructure;
using LogixDb.Cli.Commands;

namespace LogixDb.Cli.Tests;

/// <summary>
/// Helper class for creating configured CLI applications for testing purposes.
/// </summary>
public static class TestApp
{
    /// <summary>
    /// Creates and configures an instance of a CLI application for testing purposes.
    /// </summary>
    /// <param name="console">An optional custom console implementation. If not provided, a default in-memory console is used.</param>
    /// <returns>A configured instance of <see cref="CliApplication"/>.</returns>
    public static CliApplication Create<TCommand>(IConsole console) where TCommand : class, ICommand
    {
        return new CliApplicationBuilder()
            .SetTitle("Logix.Cli")
            .SetDescription("Console application providing CLI for Logix projects.")
            .SetExecutableName("logix")
            .UseConsole(console)
            .AddCommand<TCommand>()
            //.AddCommandsFrom(typeof(DbCommand).Assembly)
            .Build();
    }
}
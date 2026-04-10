using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using LogixDb.Cli.Commands;

namespace LogixDb.Cli.Tests;

/// <summary>
/// Helper class for creating configured CLI applications for testing purposes.
/// </summary>
public static class TestApp
{
    /// <summary>
    /// Creates a configured CLI application using the provided console and command descriptor.
    /// </summary>
    /// <param name="console">The console interface to be used for CLI input and output.</param>
    /// <param name="descriptor">The command descriptor that specifies the command to be added to the application.</param>
    /// <returns>An instance of <see cref="CommandLineApplication"/> configured with the specified options.</returns>
    public static CommandLineApplication Create(IConsole console, CommandDescriptor descriptor)
    {
        return new CommandLineApplicationBuilder()
            .SetTitle("Logix.Cli")
            .SetDescription("Console application providing CLI for Logix projects.")
            .SetExecutableName("logix")
            .UseConsole(console)
            .AddCommand(descriptor)
            .Build();
    }
}
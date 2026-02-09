using CliFx;
using CliFx.Infrastructure;
using LogixDb.Cli.Extensions;

namespace LogixDb.Cli;

public static class App
{
    public static async Task<int> Main()
    {
        return await new CliApplicationBuilder()
            .SetTitle("logixdb")
            .SetDescription("")
            .SetExecutableName("lgxdb")
            .UseConsole(new SystemConsole())
            .AddCommandsFromThisAssembly()
            .AddLogixServices()
            .Build()
            .RunAsync();
    }
}
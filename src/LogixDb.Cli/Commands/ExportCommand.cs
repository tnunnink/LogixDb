using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Core.Abstractions;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("export", Description = "")]
public class ExportCommand : DbCommand
{
    [CommandOption("output", 'o', Description = "")]
    public string? OutputPath { get; init; }

    [CommandOption("target", 't', Description = "")]
    public string? TargetKey { get; init; }

    [CommandOption("id", Description = "")] 
    public int SnapshotId { get; init; }

    protected override ValueTask ExecuteAsync(IConsole console, ILogixDb database)
    {
        throw new NotImplementedException();
    }
}
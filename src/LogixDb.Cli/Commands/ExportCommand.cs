using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("export", Description = "Export a target version to an L5X file")]
public partial class ExportCommand : DbCommand
{
    [Required]
    [CommandOption("output", 'o', Description = "Output path for the exported L5X file")]
    public string OutputPath { get; set; } = string.Empty;

    [Required]
    [CommandOption("target", 't', Description = "Target key to export")]
    public string TargetKey { get; set; } = string.Empty;

    [CommandOption("version", 'v', Description = "Version number of the target to export (0 for latest)")]
    public int Version { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        try
        {
            var target = await manager.GetTarget(TargetKey, Version, token);

            if (target is null)
            {
                throw new CommandException(
                    $"Target '{TargetKey}' with version {Version} not found.",
                    ErrorCodes.TargetNotFound
                );
            }

            var source = target.GetSource();
            source.Save(OutputPath);
        }
        catch (CommandException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Export failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }
}
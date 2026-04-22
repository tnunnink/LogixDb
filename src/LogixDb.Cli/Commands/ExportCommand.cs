using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data.Abstractions;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command for exporting targets to L5X files in the LogixDb CLI.
/// </summary>
/// <remarks>
/// The <see cref="ExportCommand"/> class provides functionality to export a specific target
/// to an L5X file by specifying either a target key (to export the latest instance for that target)
/// or a target ID (to export a specific target).
/// This command inherits from <see cref="DbCommand"/>, allowing database connection configuration.
/// </remarks>
/// <example>
/// This command supports the following options:
/// - Target: Exports the latest instance for the specified target key.
/// - TargetId: Exports a target with a specific ID.
/// - OutputPath: Specifies the output file path for the exported L5X file.
/// </example>
[PublicAPI]
[Command("export", Description = "Export a target to an L5X file")]
public partial class ExportCommand : DbCommand
{
    [Required]
    [CommandOption("output", 'o', Description = "Output path for the exported L5X file")]
    public string OutputPath { get; set; } = string.Empty;

    [Required]
    [CommandOption("target", 't', Description = "Target key to export")]
    public string TargetKey { get; set; } = string.Empty;

    [Required]
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
using System.ComponentModel.DataAnnotations;
using System.Xml;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using CliWrap;
using JetBrains.Annotations;
using L5Sharp.Core;
using LogixConverter.LogixSdk;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using Spectre.Console;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("import", Description = "Imports an L5X or ACD file into the database as a new target version")]
public partial class ImportCommand : DbCommand
{
    [Required]
    [CommandOption("source", 's', Description = "Path to the source L5X/ACD file to import")]
    public string? SourcePath { get; set; }

    [CommandOption("converter", Description = "Optional path to a custom ACD converter executable")]
    public string? Converter { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        if (!File.Exists(SourcePath))
            throw new CommandException($"File not found: {SourcePath}", ErrorCodes.NotFound);

        var import = Import.Create(SourcePath, SourceType.CLI);

        // Signal to the database the import is now being processed (converted, parsed, and ingested).
        import.Status = ImportStatus.Processing;
        await manager.PutImport(import, token);
        await manager.LogImport(
            import.Info($"Starting ingestion process for '{import.FileName}'"),
            token
        );

        await ConvertOrCopy(manager, import, token);
        var target = await ImportFileAsync(console, manager, import, token);
        File.Delete(import.GetTempFile());

        // Signal to the database the import process has completed
        await manager.LogImport(
            import.Info($"Import complete for target: {target.TargetKey} @v{target.VersionNumber}"),
            token
        );

        import.Status = ImportStatus.Complete;
        await manager.PutImport(import, token);
        OutputResult(console, target);
    }

    /// <summary>
    /// Imports a file into the database as a target version.
    /// </summary>
    /// <param name="console">The console instance used for output and user interaction.</param>
    /// <param name="manager">The database manager used to perform import operations.</param>
    /// <param name="import">The import details containing source file information.</param>
    /// <param name="token">A cancellation token to observe while awaiting the operation.</param>
    /// <returns>The imported target containing metadata for the processed source file.</returns>
    private static async ValueTask<Target> ImportFileAsync(IConsole console, IDbManager manager, Import import,
        CancellationToken token)
    {
        try
        {
            var result = await console.Ansi().Status().StartAsync("Importing source...", async ctx =>
            {
                ctx.Status("Loading L5X file...");
                await manager.LogImport(import.Info("Loading L5X content from disc"), token);
                var content = await L5X.LoadAsync(import.SourceFile, token);

                await manager.LogImport(import.Info("Creating new target instance for import"), token);
                var target = Target.Create(content, import.FileName);

                ctx.Status("Importing source to database...");
                await manager.LogImport(import.Info("Importing target into LogixDb database"), token);
                await manager.ImportTarget(target, token);

                return target;
            });

            return result;
        }
        catch (XmlException e)
        {
            throw new CommandException(
                $"Failed to parse L5X file with error: {e.Message}",
                ErrorCodes.FormatError,
                false, e
            );
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Database import failed with error: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }

    /// <summary>
    /// Handles the conversion or copying of a source file to a temporary location based on the file type.
    /// </summary>
    /// <param name="manager">The database manager responsible for handling import-related operations.</param>
    /// <param name="import">The import object containing details about the source file and its properties.</param>
    /// <param name="token">Cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file type of the import is unsupported.</exception>
    private async Task ConvertOrCopy(IDbManager manager, Import import, CancellationToken token)
    {
        switch (import.FileType)
        {
            case FileType.L5X:
                await CopyToTempFile(manager, import, token);
                break;
            case FileType.ACD:
                await ConvertToTempFile(manager, import, token);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(import), import.FileType, "Unsupported file type.");
        }
    }

    /// <summary>
    /// Converts an ACD file to an L5X formatted temporary file for processing.
    /// </summary>
    /// <param name="manager">The database manager instance used for logging and file operations.</param>
    /// <param name="import">The import object containing the file information and metadata.</param>
    /// <param name="token">The cancellation token used to propagate the cancellation request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ConvertToTempFile(IDbManager manager, Import import, CancellationToken token)
    {
        await manager.LogImport(import.Info("Converting file to L5X for processing"), token);

        // Use the configured ACD converter on the local machine instead of the default file converter.
        if (Converter is not null)
        {
            await manager.LogImport(import.Info("Custom ACD converter detected - calling convert command"), token);

            await CliWrap.Cli.Wrap(Converter)
                .WithArguments(args => args
                    .Add("convert")
                    .Add("-i").Add(import.SourceFile)
                    .Add("-o").Add(import.GetTempFile())
                    .Add("--force"))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(token);

            return;
        }


        // Fall back the default file converter
        await manager.LogImport(import.Info("Attempting to convert ACD using Logix SDK on local machine"), token);
        var converter = new LogixSdkConverter();
        await converter.ConvertAsync(import.SourceFile, import.GetTempFile(), token: token);
    }

    /// <summary>
    /// Creates a temporary copy of the specified import file for processing.
    /// </summary>
    /// <param name="manager">The database manager responsible for handling logging and database operations.</param>
    /// <param name="import">The import object containing information about the file to be copied.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task CopyToTempFile(IDbManager manager, Import import, CancellationToken token)
    {
        await manager.LogImport(import.Info("Creating temp L5X copy for processing"), token);
        await using var reader = File.OpenRead(import.SourceFile);
        await using var writer = File.Create(import.GetTempFile());
        await reader.CopyToAsync(writer, token);
    }

    /// <summary>
    /// Outputs the details of a target result to the console in a tabular format.
    /// </summary>
    /// <param name="console">The console instance used to write the output.</param>
    /// <param name="target">The target result containing the details to display.</param>
    private static void OutputResult(IConsole console, Target target)
    {
        var table = new Table().Border(TableBorder.Rounded).AddColumn("Property").AddColumn("Value");

        table.AddRow("Key", target.TargetKey);
        table.AddRow("Type", target.TargetType);

        if (target.TargetName is not null)
            table.AddRow("Name", target.TargetName);

        if (target.TargetCount is not null)
            table.AddRow("Count", target.TargetCount.ToString() ?? "0");

        table.AddRow("Version", target.VersionNumber.ToString());
        table.AddRow("Revision", target.SoftwareRevision ?? "?");
        table.AddRow("Date", target.ImportDate.ToString("yyyy-MM-dd HH:mm:ss"));
        table.AddRow("User", target.ImportUser);
        table.AddRow("Machine", target.ImportMachine);
        table.AddRow("Hash", target.SourceHash);

        console.Ansi().MarkupLine("[green]✓[/] Target imported successfully");
        console.Ansi().Write(table);
    }
}
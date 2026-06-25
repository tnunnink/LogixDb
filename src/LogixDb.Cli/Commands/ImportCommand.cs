using System.ComponentModel.DataAnnotations;
using System.Xml;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using LogixDb.Cli.Common;
using LogixDb.Data;
using Spectre.Console;

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
    protected override async ValueTask ExecuteAsync(IConsole console, CancellationToken token)
    {
        if (!File.Exists(SourcePath))
            throw new CommandException($"File not found: {SourcePath}", ErrorCodes.NotFound);

        var manager = GetManager();
        using var import = Import.Create(SourcePath, SourceType.CLI);

        try
        {
            // First test that we can log to the target database.
            // If not, abort the operations by throwing a command exception which will redirect to console output.
            await manager.CreateImport(import, token);
            await manager.LogImport(import.Info($"Starting ingestion process for '{SourcePath}'"), token);
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Failed to initiate import with target database: {Connection}",
                ErrorCodes.SystemError,
                false, e
            );
        }

        try
        {
            var result = await console.Ansi().Status().StartAsync("Importing source...", async ctx =>
            {
                // Signal to the database the import is now being processed (converted, parsed, and ingested).
                await manager.MarkImport(import.ImportId, ImportStatus.Processing, token);

                // Copy provided source path to local ingestion file.
                await manager.LogImport(import.Info("Writing source content to ingestion file"), token);
                await import.WriteAsync(SourcePath, token);

                // Have the import load the source, converting to L5X if needed, and return the content for processing.
                await manager.LogImport(import.Info("Converting and loading source file for processing"), token);
                ctx.Status("Converting and loading source file...");
                var content = await import.LoadAsync(new ImportConverter(Converter), token);

                // Creat a new target instance to ingest into the database
                await manager.LogImport(import.Info("Creating new target instance for import"), token);
                var target = Target.Create(content, import.FileName);

                // Run the import for the new target instance
                ctx.Status("Importing source to database...");
                await manager.LogImport(import.Info("Importing target into database"), token);
                await manager.ImportTarget(target, token);

                // Signal to the database the import process has completed
                await manager.MarkImport(import.ImportId, ImportStatus.Complete, token);
                await manager.LogImport(
                    import.Info($"Import complete for target: {target.TargetKey} @v{target.VersionNumber}"),
                    token
                );

                return target;
            });

            OutputResult(console, result);
        }
        catch (XmlException e)
        {
            await manager.MarkImport(import.ImportId, ImportStatus.Failed, token);
            await manager.LogImport(import.Error("Failed to parse L5X file during import operation", e), token);

            throw new CommandException(
                $"Failed to parse L5X file with error: {e.Message}",
                ErrorCodes.FormatError,
                false, e
            );
        }
        catch (Exception e)
        {
            await manager.MarkImport(import.ImportId, ImportStatus.Failed, token);
            await manager.LogImport(import.Error("Internal error encountered during import operation", e), token);

            throw new CommandException(
                $"Database import failed for {import.FileName}.{import.FileType}: {e.Message}",
                ErrorCodes.InternalError,
                false, e
            );
        }
    }

    /// <summary>
    /// Outputs the details of a target result to the console in a tabular format.
    /// </summary>
    /// <param name="console">The console instance used to write the output.</param>
    /// <param name="target">The target result containing the details to display.</param>
    private static void OutputResult(IConsole console, Target target)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Key", target.TargetKey);
        table.AddRow("Version", target.VersionNumber.ToString());
        table.AddRow("Type", target.TargetType);
        if (target.TargetName is not null) table.AddRow("Name", target.TargetName);
        if (target.TargetCount is not null) table.AddRow("Count", target.TargetCount.ToString() ?? "0");
        table.AddRow("Revision", target.SoftwareRevision ?? "?");
        table.AddRow("Date", target.ImportDate.ToString("yyyy-MM-dd HH:mm:ss"));
        table.AddRow("User", target.ImportUser);
        table.AddRow("Machine", target.ImportMachine);

        console.Ansi().MarkupLine("[green]✓[/] Target imported successfully");
        console.Ansi().Write(table);
    }
}
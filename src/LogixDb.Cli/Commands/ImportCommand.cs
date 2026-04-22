using System.ComponentModel.DataAnnotations;
using System.Xml;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using CliWrap;
using JetBrains.Annotations;
using L5Sharp.Core;
using LogixDb.Cli.Common;
using LogixDb.Data;
using LogixDb.Data.Abstractions;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

/// <summary>
/// Represents a command to import an L5X file as a new target into the database.
/// </summary>
[PublicAPI]
[Command("import", Description = "Imports an L5X file as a new target into the database")]
public partial class ImportCommand : DbCommand
{
    [Required]
    [CommandOption("source", 's', Description = "Path to the source L5X file to add")]
    public string? SourcePath { get; set; }

    [CommandOption("target", 't', Description = "Optional target key override (default: <TargetType>://<TargetName>)")]
    public string? TargetKey { get; set; }

    [CommandOption("converter", Description = "Optional path to a custom ACD to L5X converter executable")]
    public string? Converter { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        if (!File.Exists(SourcePath))
            throw new CommandException($"File not found: {SourcePath}", ErrorCodes.FileNotFound);

        if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(SourcePath), ".acd"))
        {
            var tempSource = await ConvertFileAsync(console, Path.GetFullPath(SourcePath), token);
            await ImportFileAsync(console, manager, tempSource, token);
            File.Delete(tempSource);
            return;
        }

        await ImportFileAsync(console, manager, Path.GetFullPath(SourcePath), token);
    }

    /// <summary>
    /// Imports a specified L5X file into the database as a new target.
    /// </summary>
    private async ValueTask ImportFileAsync(IConsole console, IDbManager database, string importTarget,
        CancellationToken token)
    {
        try
        {
            var result = await console.Ansi().Status().StartAsync("Importing source...", async ctx =>
            {
                ctx.Status("Loading L5X file...");
                var content = await L5X.LoadAsync(importTarget, token);
                var target = Target.Create(content, TargetKey);
                ctx.Status("Importing source to database...");
                await database.ImportTarget(target, token);
                return target;
            });

            OutputResult(console, result);
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
    /// Converts an ACD file to a specified format and returns the path of the converted file.
    /// </summary>
    /// <param name="console">The console interface used for displaying progress and messages.</param>
    /// <param name="sourcePath">The path to the source ACD file to be converted.</param>
    /// <param name="token">A token for observing cancellation requests.</param>
    /// <returns>The path to the converted file.</returns>
    private async ValueTask<string> ConvertFileAsync(IConsole console, string sourcePath,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Converter))
            throw new CommandException(
                "A converter path must be specified to convert ACD files. Use the --converter option to specify the path to the ACD to L5X converter executable.",
                ErrorCodes.UsageError
            );

        var destination = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}{Path.GetFileNameWithoutExtension(sourcePath)}.L5X"
        );

        try
        {
            await console.Ansi().Status().StartAsync("Converting ACD file...", _ =>
                CliWrap.Cli.Wrap(Converter)
                    .WithArguments(args => args
                        .Add("convert")
                        .Add("-i").Add(sourcePath)
                        .Add("-o").Add(destination)
                        .Add("--force"))
                    .WithValidation(CommandResultValidation.ZeroExitCode)
                    .ExecuteAsync(token)
            );

            return destination;
        }
        catch (CommandException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new CommandException(
                $"Project conversion failed with error: {e.Message}",
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
        var table = new Table().Border(TableBorder.Rounded).AddColumn("Property").AddColumn("Value");

        table.AddRow("Key", target.TargetKey);
        table.AddRow("Type", target.TargetType);
        table.AddRow("Name", target.TargetName);
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
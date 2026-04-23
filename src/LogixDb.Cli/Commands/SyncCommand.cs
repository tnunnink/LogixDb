using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using LogixDb.Data.Abstractions;
using LogixDb.Cli.Common;
using JetBrains.Annotations;
using L5Sharp.Core;
using L5Sharp.Gateway;
using L5Sharp.Gateway.Extensions;
using LogixDb.Data;
using Spectre.Console;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("sync", Description = "Syncs tag data from an online PLC to the latest target in the database")]
public partial class SyncCommand : DbCommand
{
    [Required]
    [CommandOption("target", 't', Description = "The target key of the project to sync")]
    public string TargetKey { get; set; } = string.Empty;

    [CommandOption("ip", Description = "Processor IP address. Defaults to comm path from source project.")]
    public string? IpAddress { get; set; }

    [CommandOption("slot", Description = "Processor slot number. Defaults to  comm path from source project.")]
    public int Slot { get; set; } = -1;

    protected override async ValueTask ExecuteAsync(IConsole console, IDbManager manager, CancellationToken token)
    {
        var target = await manager.GetTarget(TargetKey, token: token);

        if (target is null)
            throw new CommandException($"No target found for target key: {TargetKey}", ErrorCodes.FileNotFound);

        var source = target.GetSource();
        var ipAddress = IpAddress ?? ParseIpAddress(source);
        var slotNumber = Slot >= 0 ? (ushort)Slot : ParseSlotNumber(source);

        await console.Ansi().Status().StartAsync($"Syncing tags from {ipAddress} slot {slotNumber}...", async ctx =>
        {
            ctx.Status("Uploading tag data from PLC...");
            using var client = new PlcClient(ipAddress, slotNumber);
            await source.Upload(client, token);

            ctx.Status("Creating new target...");
            var updated = Target.Create(source, TargetKey);

            ctx.Status("Adding target to database...");
            await manager.ImportTarget(updated, token);
        });

        console.Ansi().MarkupLine("[green]✓[/] Successfully synced tag data from PLC.");
    }

    /// <summary>
    /// Determines the IP address from the given L5X source object.
    /// </summary>
    /// <param name="source">The source object containing the controller communication path.</param>
    /// <returns>The extracted IP address as a string.</returns>
    /// <exception cref="CommandException">
    /// Thrown when the communication path is not configured in the latest target.
    /// </exception>
    private static string ParseIpAddress(L5X source)
    {
        if (source.Controller.CommPath is null)
            throw new CommandException(
                "Communication path is not configured in the latest target.",
                ErrorCodes.UsageError
            );

        // I'm not sure if this format can change or what the actual structure is. Will need to research that.
        // Driver\IP\Backplane\Slot
        var parts = source.Controller.CommPath.Split("\\");

        if (parts.Length != 4)
            throw new CommandException(
                @"Invalid communication path format. Expected format: Driver\IP\Backplane\Slot",
                ErrorCodes.FormatError
            );

        return parts[1];
    }

    /// <summary>
    /// Extracts the slot number from the controller communication path in the provided L5X source object.
    /// </summary>
    /// <param name="source">The source object containing the controller communication path.</param>
    /// <returns>The extracted slot number as an integer.</returns>
    /// <exception cref="CommandException">
    /// Thrown when the communication path is not configured in the latest target.
    /// </exception>
    private static ushort ParseSlotNumber(L5X source)
    {
        if (source.Controller.CommPath is null)
            throw new CommandException(
                "Communication path is not configured in the latest target.", ErrorCodes.UsageError);

        // I'm not sure if this format can change or what the actual structure is. Will need to research that.
        // Driver\IP\Backplane\Slot
        var parts = source.Controller.CommPath.Split("\\");

        if (parts.Length != 4)
            throw new CommandException(
                @"Invalid communication path format. Expected format: Driver\IP\Backplane\Slot",
                ErrorCodes.FormatError
            );

        return ushort.Parse(parts[3]);
    }
}
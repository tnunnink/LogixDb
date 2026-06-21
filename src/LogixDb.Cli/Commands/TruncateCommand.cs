using System.ComponentModel.DataAnnotations;
using System.Text;
using CliFx.Binding;
using CliFx.Infrastructure;
using JetBrains.Annotations;

namespace LogixDb.Cli.Commands;

[PublicAPI]
[Command("truncate", Description = "Delete old versions of a specified target")]
public partial class TruncateCommand : DbCommand
{
    [Required]
    [CommandOption("target", 't', Description = "Target key to purge")]
    public string Target { get; set; } = string.Empty;

    [CommandOption("before-version", Description = "Delete all versions before specified version number")]
    public int? BeforeVersion { get; set; }

    [CommandOption("before-date", Description = "Delete all versions before specified date")]
    public DateTime? BeforeDate { get; set; }

    [CommandOption("force", 'f', Description = "Skip confirmation prompt.")]
    public bool Force { get; set; }

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(IConsole console, CancellationToken token)
    {
        if (!ValidateOptions(console)) return;

        // Show confirmation prompt unless Force is set
        if (!Force)
        {
            var message = BuildConfirmationMessage();
            await console.Output.WriteLineAsync(message.ToString());
            await console.Output.WriteAsync("Are you sure you want to proceed? (y/n): ");

            var response = await console.Input.ReadLineAsync(token);
            if (!string.Equals(response?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
            {
                await console.Output.WriteLineAsync("Operation cancelled.");
                return;
            }
        }

        var manager = GetManager();

        if (BeforeDate.HasValue)
        {
            await manager.DeleteVersions(Target, BeforeDate.Value, token);
        }

        if (BeforeVersion.HasValue)
        {
            await manager.DeleteVersions(Target, BeforeVersion.Value, token);
        }


        await console.Output.WriteLineAsync($"");
    }

    /// <summary>
    /// Constructs a confirmation message based on the specified options.
    /// This message includes details about the target key and any conditions
    /// such as truncation before a specific version or date.
    /// </summary>
    /// <returns>
    /// A <see cref="StringBuilder"/> containing the confirmation message
    /// that describes the truncation operation.
    /// </returns>
    private StringBuilder BuildConfirmationMessage()
    {
        var message = new StringBuilder();

        message.Append($"Truncate entries for key '{Target}'");

        if (BeforeVersion.HasValue)
        {
            message.Append($" before version {BeforeVersion.Value}");
        }

        if (BeforeDate.HasValue)
        {
            message.Append($" before date {BeforeDate.Value:yyyy-MM-dd HH:mm:ss}");
        }

        return message;
    }

    /// <summary>
    /// Validates the command options to ensure required conditions are met.
    /// This includes checking that at least one filter is provided and validating
    /// that mutually exclusive options are not used together.
    /// </summary>
    /// <param name="console">
    /// An instance of <see cref="IConsole"/> used to display error messages if validation fails.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the options are valid.
    /// Returns <c>true</c> if the options are valid; otherwise, <c>false</c>.
    /// </returns>
    private bool ValidateOptions(IConsole console)
    {
        // Validate that at least one filter is provided
        if (BeforeVersion is null && BeforeDate is null)
        {
            console.Error.WriteLine("Either --before-version or --before-date must be specified.");
            return false;
        }

        // Validate that both filters are not provided
        if (BeforeVersion is not null && BeforeDate is not null)
        {
            console.Error.WriteLine("Only one of --before-version or --before-date can be specified, not both.");
            return false;
        }

        return true;
    }
}
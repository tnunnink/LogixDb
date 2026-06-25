using CliWrap;
using LogixConverter.Abstractions;
using LogixConverter.LogixSdk;

namespace LogixDb.Data;

/// <summary>
/// Represents a converter for importing files, using an external executable
/// or a fallback mechanism for file conversion.
/// </summary>
public class ImportConverter(string? executable = null) : ILogixFileConverter
{
    /// <summary>
    /// A readonly instance of LogixSdkConverter used as a fallback mechanism
    /// for file conversion when no external executable is provided.
    /// Used by the ImportConverter class when the primary method of
    /// conversion through an external tool is unavailable.
    /// </summary>
    private readonly LogixSdkConverter _fallback = new();

    /// <summary>
    /// Converts a Logix file from one format to another asynchronously.
    /// If an external executable is configured, it will be used for conversion;
    /// otherwise, it falls back to the LogixSdkConverter.
    /// </summary>
    /// <param name="filePath">The path to the source file to be converted.</param>
    /// <param name="savePath">The path where the converted file should be saved.</param>
    /// <param name="options">Optional conversion options to customize the conversion process.</param>
    /// <param name="token">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a ConversionResult with details about the conversion outcome.</returns>
    public async Task<ConversionResult> ConvertAsync(string filePath, string savePath,
        ConversionOptions? options = null,
        CancellationToken token = default
    )
    {
        if (executable is not null)
        {
            var result = await Cli.Wrap(executable)
                .WithArguments(args => args
                    .Add("convert")
                    .Add("-i").Add(filePath)
                    .Add("-o").Add(savePath)
                    .Add("--force"))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(token);

            return new ConversionResult(result.IsSuccess, filePath, savePath, result.RunTime, DateTime.Now);
        }

        return await _fallback.ConvertAsync(filePath, savePath, options, token);
    }
}
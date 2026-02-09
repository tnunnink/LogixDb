using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using CliFx.Infrastructure;
using LogixDb.Cli.Common;
using Spectre.Console;

namespace LogixDb.Cli.Extensions;

/// <summary>
/// 
/// </summary>
public static class GeneralExtensions
{
    /// <summary>
    /// Creates an instance of <see cref="IAnsiConsole"/> using the specified console
    /// output, applying settings for ANSI support and color system detection.
    /// </summary>
    /// <param name="console">
    /// The console to use for output. This represents the underlying console interface
    /// where ANSI-rendered output will be written.
    /// </param>
    /// <returns>An instance of <see cref="IAnsiConsole"/> configured with the specified settings.</returns>
    public static IAnsiConsole Ansi(this IConsole console)
    {
        return AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Out = new AnsiConsoleOutput(console.Output)
        });
    }

    /// <summary>
    /// Determines if the specified file path corresponds to a project file with an extension matching the given project type.
    /// </summary>
    /// <param name="path">The file path to check for a valid project file extension.</param>
    /// <param name="type">The type of project to verify against the file extension.</param>
    /// <returns>True if the file extension matches the specified project type, otherwise false.</returns>
    public static bool IsLogixFile(this string path, FileType type)
    {
        return string.Equals(Path.GetExtension(path), $".{type}", StringComparison.OrdinalIgnoreCase);
    }
}
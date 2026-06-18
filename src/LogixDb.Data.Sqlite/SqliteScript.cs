using System.Reflection;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Provides a collection of SQL query scripts for interacting with an SQLite database.
/// The class defines static properties representing various SQL commands
/// by loading them from embedded resource files.
/// </summary>
internal static class SqliteScript
{
    public static string DeleteTarget => Get(nameof(DeleteTarget));
    public static string DeleteVersion => Get(nameof(DeleteVersion));
    public static string DeleteVersionsBeforeDate => Get(nameof(DeleteVersionsBeforeDate));
    public static string DeleteVersionsByNumber => Get(nameof(DeleteVersionsByNumber));
    public static string GetTableNames => Get(nameof(GetTableNames));
    public static string GetTargetByLatest => Get(nameof(GetTargetByLatest));
    public static string GetTargetByVersion => Get(nameof(GetTargetByVersion));
    public static string ListTargets => Get(nameof(ListTargets));
    public static string MergeAoi => Get(nameof(MergeAoi));
    public static string MergeAoiParameter => Get(nameof(MergeAoiParameter));
    public static string MergeController => Get(nameof(MergeController));
    public static string MergeDataType => Get(nameof(MergeDataType));
    public static string MergeDataTypeMember => Get(nameof(MergeDataTypeMember));
    public static string MergeModule => Get(nameof(MergeModule));
    public static string MergeModuleConnection => Get(nameof(MergeModuleConnection));
    public static string MergeModulePort => Get(nameof(MergeModulePort));
    public static string MergeOperand => Get(nameof(MergeOperand));
    public static string MergeProgram => Get(nameof(MergeProgram));
    public static string MergeRoutine => Get(nameof(MergeRoutine));
    public static string MergeRung => Get(nameof(MergeRung));
    public static string MergeRungArgument => Get(nameof(MergeRungArgument));
    public static string MergeRungInstruction => Get(nameof(MergeRungInstruction));
    public static string MergeRungReference => Get(nameof(MergeRungReference));
    public static string MergeTag => Get(nameof(MergeTag));
    public static string MergeTagComment => Get(nameof(MergeTagComment));
    public static string MergeTagConsumer => Get(nameof(MergeTagConsumer));
    public static string MergeTagMember => Get(nameof(MergeTagMember));
    public static string MergeTagProducer => Get(nameof(MergeTagProducer));
    public static string MergeTagValue => Get(nameof(MergeTagValue));
    public static string MergeTask => Get(nameof(MergeTask));
    public static string PostInfo => Get(nameof(PostInfo));
    public static string PostLog => Get(nameof(PostLog));
    public static string PostTarget => Get(nameof(PostTarget));
    public static string PostVersion => Get(nameof(PostVersion));
    public static string PutImport => Get(nameof(PutImport));

    /// <summary>
    /// Retrieves the content of an embedded SQL script resource by its name.
    /// </summary>
    /// <param name="scriptName">The name of the SQL script to retrieve, excluding file extension.</param>
    /// <returns>The content of the requested SQL script as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified SQL script is not found as an embedded resource.</exception>
    private static string Get(string scriptName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        // The resource name usually follows the pattern: {ProjectNamespace}.{Folder}.{FileName}.sql
        var resourceName = $"LogixDb.Data.Sqlite.Scripts.{scriptName}.sql";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"SQL script '{scriptName}' not found as embedded resource.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
using System.Reflection;

namespace LogixDb.Data;

/// <summary>
/// Represents a static class that defines constants for tagging migrations in the system.
/// These tags can be used to categorize or filter migrations based on specific requirements.
/// </summary>
public static class MigrationTag
{
    /// <summary>
    /// Marks migrations that are always applied, regardless of user configuration.
    /// Required migrations typically include core schema tables and metadata tables.
    /// </summary>
    public const string Required = nameof(Required);

    /// <summary>
    /// Marks migrations related to creating or modifying controller-specific tables in the database schema.
    /// Controller migrations typically include structures necessary for managing hardware controllers and their configurations.
    /// </summary>
    public const string Controller = "controller";

    /// <summary>
    /// Marks migrations that add or modify data type-related tables in the database schema.
    /// DataType migrations typically include tables for managing user-defined data types, atomic types, and structure definitions.
    /// </summary>
    public const string DataType = "data_type";

    /// <summary>
    /// Marks migrations related to Add-On Instructions (AOI) in the database schema.
    /// AOI migrations include tables for AOI definitions, parameters, local tags, operands, and rungs.
    /// </summary>
    public const string Aoi = "aoi";

    /// <summary>
    /// Marks migrations related to module-related tables in the database schema.
    /// Module migrations typically include hardware module configurations and their associated data.
    /// </summary>
    public const string Module = "module";

    /// <summary>
    /// Marks migrations that add or modify tag-related tables in the database schema.
    /// Tag migrations typically include tables for managing controller tags, program tags, and their associated data.
    /// </summary>
    public const string Tag = "tag";

    /// <summary>
    /// Marks migrations related to program-related tables in the database schema.
    /// Program migrations include tables for managing control programs and their associated configurations.
    /// </summary>
    public const string Program = "program";

    /// <summary>
    /// Marks migrations related to routine-related tables in the database schema.
    /// Routine migrations include tables for managing code routines within programs or AOIs.
    /// </summary>
    public const string Routine = "routine";

    /// <summary>
    /// Marks migrations related to ladder rungs and logic in the database schema.
    /// Rung migrations include tables for rungs, instructions, and their arguments.
    /// </summary>
    public const string Rung = "rung";

    /// <summary>
    /// Marks migrations related to task-related tables in the database schema.
    /// Task migrations include tables for managing execution tasks and their schedules.
    /// </summary>
    public const string Task = "task";

    /// <summary>
    /// Retrieves a filtered list of migration tags based on the specified <see cref="DbOptions"/>.
    /// </summary>
    /// <param name="options">The table options used to include or exclude specific migration tags.
    /// If null or both Include and Exclude are empty, defaults to returning all component migration tags.</param>
    /// <returns>A collection of migration tags filtered according to the provided options.</returns>
    public static IEnumerable<string> GetTags(DbOptions? options)
    {
        var tags = All().ToHashSet();

        if (options is null || (options.Include.Length == 0 && options.Exclude.Length == 0))
        {
            return tags;
        }

        if (options.Include.Length > 0) tags.IntersectWith(options.Include);
        if (options.Exclude.Length > 0) tags.ExceptWith(options.Exclude);

        // just ensure that require is always present
        tags.Add(Required);
        return tags;
    }

    /// <summary>
    /// Returns a list of all migration tags defined in this class.
    /// This method uses reflection to dynamically retrieve all constant string fields.
    /// </summary>
    /// <returns>A list containing all migration tag values defined in the MigrationTag class.</returns>
    private static List<string> All()
    {
        return typeof(MigrationTag)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
    }
}
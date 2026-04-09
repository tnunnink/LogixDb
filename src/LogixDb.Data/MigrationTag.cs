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
    public const string Controller = nameof(ComponentOptions.Controller);

    /// <summary>
    /// Marks migrations that add or modify data type-related tables in the database schema.
    /// DataType migrations typically include tables for managing user-defined data types, atomic types, and structure definitions.
    /// </summary>
    public const string DataType = nameof(ComponentOptions.DataType);

    /// <summary>
    /// Marks migrations related to Add-On Instructions (AOI) in the database schema.
    /// AOI migrations include tables for AOI definitions, parameters, local tags, operands, and rungs.
    /// </summary>
    public const string Aoi = nameof(ComponentOptions.Aoi);

    /// <summary>
    /// Marks migrations related to module-related tables in the database schema.
    /// Module migrations typically include hardware module configurations and their associated data.
    /// </summary>
    public const string Module = nameof(ComponentOptions.Module);

    /// <summary>
    /// Marks migrations that add or modify tag-related tables in the database schema.
    /// Tag migrations typically include tables for managing controller tags, program tags, and their associated data.
    /// </summary>
    public const string Tag = nameof(ComponentOptions.Tag);

    /// <summary>
    /// Marks migrations related to ladder rungs and logic in the database schema.
    /// Rung migrations include tables for rungs, instructions, and their arguments.
    /// </summary>
    public const string Logic = nameof(ComponentOptions.Logic);

    /// <summary>
    /// Retrieves a collection of migration tags based on the specified component options.
    /// Combines constant tag values dynamically depending on the flags set in the provided options.
    /// </summary>
    /// <param name="options">
    /// The component options used to determine which migration tags to include.
    /// This parameter is a flags-based enumeration where multiple options can be combined.
    /// </param>
    /// <returns>
    /// An enumerable collection of strings containing the selected migration tags.
    /// </returns>
    public static IEnumerable<string> GetTags(ComponentOptions options)
    {
        var tags = new List<string> { Required };

        if (options.HasFlag(ComponentOptions.Controller))
            tags.Add(Controller);

        if (options.HasFlag(ComponentOptions.DataType))
            tags.Add(DataType);

        if (options.HasFlag(ComponentOptions.Aoi))
            tags.Add(Aoi);

        if (options.HasFlag(ComponentOptions.Module))
            tags.Add(Module);

        if (options.HasFlag(ComponentOptions.Tag))
            tags.Add(Tag);

        if (options.HasFlag(ComponentOptions.Logic))
            tags.Add(Logic);

        return tags;
    }
}
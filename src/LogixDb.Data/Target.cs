using System.Data;
using System.Xml.Linq;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Extensions;
using LogixDb.Data.Transformers;

namespace LogixDb.Data;

/// <summary>
/// Represents a Target of an L5X file containing Logix controller data.
/// This class captures metadata about the export, including target information, schema details,
/// and maintains both a hash and the compressed XML source data for versioning and retrieval purposes.
/// Targets are persisted to the database and linked to a target through the Target table.
/// </summary>
public sealed class Target
{
    /// <summary>
    /// A private readonly dictionary mapping string identifiers to their respective implementations of the
    /// <see cref="IDbTransformer"/> interface. This field serves as a central repository for
    /// transformers responsible for processing and transforming various types of Logix database entities,
    /// such as controllers, data types, AOIs, operands, and more. The keys in the dictionary are case-insensitive,
    /// allowing for robust and flexible access.
    /// </summary>
    private readonly List<IDbTransformer> _transformers =
    [
        new ControllerTransformer(),
        new DataTypeTransformer(),
        new AoiTransformer(),
        new OperandTransformer(),
        new ModuleTransformer(),
        new TaskTransformer(),
        new ProgramTransformer(),
        new RoutineTransformer(),
        new RungTransformer(),
        new TagTransformer()
    ];

    /// <summary>
    /// A private field representing the parsed L5X data associated with the Target.
    /// This field is lazily initialized when the source data is decompressed and parsed,
    /// and it serves as the in-memory representation of the XML data from the L5X file.
    /// </summary>
    private L5X? _pristine;

    /// <summary>
    /// A private nullable field containing a scrubbed instance of the <see cref="L5X"/> class.
    /// The scrubbed instance represents a version of the target's data with sensitive or unnecessary
    /// details removed for security or data sanitation purposes. This field is lazily populated during
    /// retrieval when the scrubbed version of the source is requested.
    /// </summary>
    private L5X? _scrubbed;

    public string TargetKey { get; init; } = string.Empty;
    public int VersionId { get; set; }
    public int VersionNumber { get; set; }
    public string TargetType { get; init; } = string.Empty;
    public string? TargetName { get; init; }
    public int? TargetCount { get; init; }
    public bool IsPartial { get; init; }
    public string? SchemaRevision { get; init; }
    public string? SoftwareRevision { get; init; }
    public DateTime? ExportDate { get; init; } = DateTime.MinValue;
    public string? ExportOptions { get; init; }
    public DateTime ImportDate { get; init; } = DateTime.Now;
    public string ImportUser { get; init; } = Environment.UserName;
    public string ImportMachine { get; init; } = Environment.MachineName;
    public string SourceHash { get; init; } = string.Empty;
    public byte[] SourceData { get; init; } = [];
    public Dictionary<string, string> Info { get; } = [];

    /// <summary>
    /// Creates a new Target instance from an L5X source file.
    /// Extracts metadata from the L5X content and generates a hash of the serialized source data.
    /// </summary>
    /// <param name="source">The L5X source file containing Logix controller data for which to take a Target.</param>
    /// <param name="targetKey">An optional custom target key. If not provided, a default key is generated from the
    ///     target type and name in the format "targettype://targetname".</param>
    /// <returns>A new Target instance populated with metadata and source data from the L5X file.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the L5X content does not contain a valid TargetType or TargetName.
    /// </exception>
    public static Target Create(L5X source, string targetKey)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(targetKey);

        if (source.Content.TargetType is null)
            throw new InvalidOperationException(
                "The L5X content does not contain a valid TargetType. Cannot create Target without a target type.");

        if (source.Content.TargetName is null)
            throw new InvalidOperationException(
                "The L5X content does not contain a valid TargetName. Cannot create Target without a target name.");

        return new Target
        {
            TargetKey = targetKey,
            TargetType = source.Content.TargetType,
            TargetName = source.Content.TargetName,
            TargetCount = source.Content.TargetCount,
            IsPartial = source.Content.ContainsContext,
            SchemaRevision = source.Content.SchemaRevision,
            SoftwareRevision = source.Content.SoftwareRevision,
            ExportDate = source.Content.ExportDate,
            ExportOptions = string.Join(",", source.Content.ExportOptions),
            SourceHash = source.Content.HashElement(),
            SourceData = source.Content.Serialize().ToString().Compress(),
            _pristine = source
        };
    }

    /// <summary>
    /// Retrieves the parsed L5X source data associated with the Target.
    /// If the parsed data has not yet been initialized, it is created by decompressing and parsing
    /// the stored source data.
    /// </summary>
    /// <returns>The parsed L5X instance representing the source data of the Target.</returns>
    public L5X GetSource(bool scrub = false)
    {
        if (scrub)
            return _scrubbed ??= ScrubData(L5X.Parse(SourceData.Decompress()));

        return _pristine ??= L5X.Parse(SourceData.Decompress());
    }

    /// <summary>
    /// Compiles the target's data by applying each registered transformer to generate a collection of data tables.
    /// Each transformer processes the target and produces one or more data tables which are then aggregated into the result.
    /// </summary>
    /// <returns>A collection of data tables populated with the transformed data from the target.</returns>
    public IEnumerable<DataTable> Compile()
    {
        var data = new List<DataTable>();

        foreach (var transformer in _transformers)
        {
            var tables = transformer.Transform(this);
            data.AddRange(tables);
        }

        return data;
    }

    /// <summary>
    /// Returns a string representation of the Target instance.
    /// The representation includes the target type and target name in a standardized format.
    /// </summary>
    /// <returns>A string in the format "targettype://targetname".</returns>
    public override string ToString() => TargetKey;

    /// <summary>
    /// Cleans the given L5X source content by removing L5K-formatted data within tags.
    /// This is just noise for the system and can cause duplication issues when hashing tag element structures.
    /// </summary>
    /// <param name="source">The source L5X data object that will be processed and scrubbed.</param>
    /// <returns>A new instance of the L5X class with the scrubbed content, preserving all other information.</returns>
    private static L5X ScrubData(L5X source)
    {
        var content = source.Content.Serialize();

        content.Descendants(L5XName.Data)
            .Where(e => e.Attribute(L5XName.Format)?.Value == DataFormat.L5K)
            .Remove();

        return new L5X(new LogixContent(content));
    }
}
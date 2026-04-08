using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Transformers;

namespace LogixDb.Data;

/// <summary>
/// Represents a snapshot of an L5X file containing Logix controller data.
/// This class captures metadata about the export, including target information, schema details,
/// and maintains both a hash and the compressed XML source data for versioning and retrieval purposes.
/// Snapshots are persisted to the database and linked to a target through the snapshot table.
/// </summary>
public sealed class Snapshot
{
    /// <summary>
    /// A private readonly dictionary mapping string identifiers to their respective implementations of the
    /// <see cref="ISnapshotTransformer"/> interface. This field serves as a central repository for
    /// transformers responsible for processing and transforming various types of Logix database entities,
    /// such as controllers, data types, AOIs, operands, and more. The keys in the dictionary are case-insensitive,
    /// allowing for robust and flexible access.
    /// </summary>
    private readonly Dictionary<string, ISnapshotTransformer> _transformers =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "controller", new ControllerTransformer() },
            { "data_type", new DataTypeTransformer() },
            { "aoi", new AoiTransformer() },
            { "operand", new OperandTransformer() },
            { "module", new ModuleTransformer() },
            { "task", new TaskTransformer() },
            { "program", new ProgramTransformer() },
            { "routine", new RoutineTransformer() },
            { "rung", new RungTransformer() },
            { "tag", new TagTransformer() }
        };

    /// <summary>
    /// A private field representing the parsed L5X data associated with the snapshot.
    /// This field is lazily initialized when the source data is decompressed and parsed,
    /// and it serves as the in-memory representation of the XML data from the L5X file.
    /// </summary>
    private L5X? _l5X;

    public int SnapshotId { get; set; }
    public string TargetKey { get; init; } = string.Empty;
    public string TargetType { get; init; } = string.Empty;
    public string TargetName { get; init; } = string.Empty;
    public bool IsPartial { get; init; }
    public string? SchemaRevision { get; init; }
    public string? SoftwareRevision { get; init; }
    public DateTime? ExportDate { get; init; } = DateTime.MinValue;
    public string? ExportUser { get; init; }
    public string? ExportOptions { get; init; }
    public DateTime ImportDate { get; init; } = DateTime.Now;
    public string ImportUser { get; init; } = Environment.UserName;
    public string ImportMachine { get; init; } = Environment.MachineName;
    public string SourceHash { get; init; } = string.Empty;
    public byte[] SourceData { get; init; } = [];
    public Dictionary<string, string> Metadata { get; } = [];

    /// <summary>
    /// Creates a new snapshot instance from an L5X source file.
    /// Extracts metadata from the L5X content and generates a hash of the serialized source data.
    /// </summary>
    /// <param name="source">The L5X source file containing Logix controller data for which to take a snapshot.</param>
    /// <param name="targetKey">An optional custom target key. If not provided, a default key is generated from the
    /// target type and name in the format "targettype://targetname".</param>
    /// <returns>A new Snapshot instance populated with metadata and source data from the L5X file.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the L5X content does not contain a valid TargetType or TargetName.
    /// </exception>
    public static Snapshot Create(L5X source, string? targetKey = null)
    {
        if (source.Content.TargetType is null)
            throw new InvalidOperationException(
                "The L5X content does not contain a valid TargetType. Cannot create snapshot without a target type.");

        if (source.Content.TargetName is null)
            throw new InvalidOperationException(
                "The L5X content does not contain a valid TargetName. Cannot create snapshot without a target name.");

        return new Snapshot
        {
            TargetKey = targetKey ?? $"{source.Content.TargetType.ToLower()}://{source.Content.TargetName}",
            TargetType = source.Content.TargetType,
            TargetName = source.Content.TargetName,
            IsPartial = source.Content.ContainsContext,
            SchemaRevision = source.Content.SchemaRevision,
            SoftwareRevision = source.Content.SoftwareRevision,
            ExportDate = source.Content.ExportDate,
            ExportUser = source.Content.Owner,
            ExportOptions = string.Join(",", source.Content.ExportOptions),
            SourceHash = source.Content.Serialize().ToString().Hash().ToHexString(),
            SourceData = source.Content.Serialize().ToString().Compress(),
            _l5X = source
        };
    }

    /// <summary>
    /// Retrieves the parsed L5X source data associated with the snapshot.
    /// If the parsed data has not yet been initialized, it is created by decompressing and parsing
    /// the stored source data.
    /// </summary>
    /// <returns>The parsed L5X instance representing the source data of the snapshot.</returns>
    public L5X GetSource() => _l5X ??= L5X.Parse(SourceData.Decompress());

    /// <summary>
    /// Compiles the snapshot's data into a collection of DataTables filtered by the given table names.
    /// Filters the data transformers based on the provided table names, applies the transformations,
    /// and yields only the DataTables matching the specified names.
    /// </summary>
    /// <param name="tableNames">A collection of table names to filter the data during the compilation process.</param>
    /// <returns>An enumerable collection of DataTable objects that match the specified table names.</returns>
    public IEnumerable<DataTable> Compile(ICollection<string> tableNames)
    {
        // Filter transformers based on provided table options.
        var transformers = _transformers.Where(kvp => tableNames.Contains(kvp.Key)).Select(kvp => kvp.Value);

        foreach (var transformer in transformers)
        {
            var tables = transformer.Transform(this);

            // Further filter specific DataTables based on what actually exists in the DB schema
            foreach (var table in tables)
                if (tableNames.Contains(table.TableName))
                    yield return table;
        }
    }

    /// <summary>
    /// Returns a string representation of the snapshot instance.
    /// The representation includes the target type and target name in a standardized format.
    /// </summary>
    /// <returns>A string in the format "targettype://targetname".</returns>
    public override string ToString() => TargetKey;
}
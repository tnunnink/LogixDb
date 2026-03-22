using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "tag_produce_info" table, defining how the
/// fields in the <see cref="TagProduceInfoRecord"/> are associated with the columns in the database table.
/// This class is responsible for specifying column mappings, their characteristics, and the table name.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="TableMap{T}"/>, and provides the implementation specific to
/// the structure of <see cref="TagProduceInfoRecord"/>. Each property in the record is mapped to a database column,
/// including metadata regarding its hashability and data transformation if applicable.
/// </remarks>
public class TagProduceInfoMap : TableMap<TagProduceInfoRecord>
{
    /// <inheritdoc />
    public override string TableName => "tag_produce_info";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<TagProduceInfoRecord>> Columns =>
    [
        ColumnMap<TagProduceInfoRecord>.For(r => r.ProduceInfoId, "porduce_info_id", hashable: false),
        ColumnMap<TagProduceInfoRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<TagProduceInfoRecord>.For(r => r.TagId, "tag_id"),
        ColumnMap<TagProduceInfoRecord>.For(r => r.ProduceInfo.ProduceCount, "produce_count"),
        ColumnMap<TagProduceInfoRecord>.For(r => r.ProduceInfo.ProgrammaticallySendEventTrigger,
            "programatically_send_event_trigger"),
        ColumnMap<TagProduceInfoRecord>.For(r => r.ProduceInfo.UnicastPermitted, "unicast_permitted"),
        ColumnMap<TagProduceInfoRecord>.For(r => (float)r.ProduceInfo.MaximumRPI, "maximum_rpi"),
        ColumnMap<TagProduceInfoRecord>.For(r => (float)r.ProduceInfo.MinimumRPI, "minimum_rpi"),
        ColumnMap<TagProduceInfoRecord>.For(r => (float)r.ProduceInfo.DefaultRPI, "default_rpi")
    ];
}

/// <summary>
/// Represents a record containing produce information associated with a tag.
/// This record includes details such as the snapshot identifier, tag identifier,
/// and produce-related metadata encapsulated in the <see cref="ProduceInfo"/> object.
/// </summary>
/// <param name="SnapshotId">The identifier for the snapshot associated with the produce information.</param>
/// <param name="TagId">The unique identifier for the tag associated with the produce information.</param>
/// <param name="ProduceInfo">An object containing detailed information about produce-related settings and data.</param>
public record TagProduceInfoRecord(int SnapshotId, Guid TagId, ProduceInfo ProduceInfo)
{
    public Guid ProduceInfoId { get; } = Guid.NewGuid();
}
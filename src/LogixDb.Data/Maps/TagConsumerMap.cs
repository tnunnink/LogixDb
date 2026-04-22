using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "tag_consumer" table, defining how the
/// fields in the <see cref="TagConsumeInfoRecord"/> are associated with the columns in the database table.
/// </summary>
internal class TagConsumerMap : TableMap<TagConsumeInfoRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_consumer";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagConsumeInfoRecord>> Columns =>
    [
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumerId, "consumer_id", hashable: false),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.InstanceId, "instance_id", hashable: false),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.TagId, "tag_id", hashable: false),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.Producer, "producer"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.RemoteTag, "remote_tag"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.RemoteInstance, "remote_instance"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.RPI, "rpi"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.Unicast, "unicast"),
        ColumnMap<TagConsumeInfoRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a record containing consume information associated with a tag.
/// </summary>
/// <param name="TagId">The unique identifier for the tag associated with the consumer information.</param>
/// <param name="ConsumeInfo">An object containing detailed information about consume-related settings and data.</param>
internal record TagConsumeInfoRecord(int InstanceId, Guid TagId, ConsumeInfo ConsumeInfo)
{
    public Guid ConsumerId { get; } = Guid.NewGuid();
}
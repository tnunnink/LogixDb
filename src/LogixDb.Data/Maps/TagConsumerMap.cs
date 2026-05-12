using L5Sharp.Core;

namespace LogixDb.Data.Maps;

internal class TagConsumerMap : TableMap<ConsumeInfo>
{
    /// <inheritdoc />
    protected override string TableName => "tag_consumer";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ConsumeInfo>> Columns =>
    [
        ColumnMap<ConsumeInfo>.For(r => r.Metadata.Get<string>("tag_hash"), "tag_hash"),
        ColumnMap<ConsumeInfo>.For(r => r.Producer, "producer"),
        ColumnMap<ConsumeInfo>.For(r => r.RemoteTag, "remote_tag"),
        ColumnMap<ConsumeInfo>.For(r => r.RemoteInstance, "remote_instance"),
        ColumnMap<ConsumeInfo>.For(r => r.RPI, "rpi"),
        ColumnMap<ConsumeInfo>.For(r => r.Unicast, "unicast"),
        ColumnMap<ConsumeInfo>.For(ComputeHash, "record_hash")
    ];
}
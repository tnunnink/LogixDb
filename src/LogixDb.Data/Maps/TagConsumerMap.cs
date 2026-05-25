using L5Sharp.Core;

namespace LogixDb.Data.Maps;

public class TagConsumerMap : TableMap<ConsumeInfo>
{
    /// <inheritdoc />
    protected override string TableName => "tag_consumer";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ConsumeInfo>> Columns =>
    [
        ColumnMap<ConsumeInfo>.For(r => r.GetParent<Tag>()?.Metadata.Get<string>("record_hash"), "tag_hash", false),
        ColumnMap<ConsumeInfo>.For(r => r.Producer, "producer"),
        ColumnMap<ConsumeInfo>.For(r => r.RemoteTag, "remote_tag"),
        ColumnMap<ConsumeInfo>.For(r => r.RemoteInstance, "remote_instance"),
        ColumnMap<ConsumeInfo>.For(r => r.RPI, "rpi"),
        ColumnMap<ConsumeInfo>.For(r => r.Unicast, "unicast"),
        ColumnMap<ConsumeInfo>.RecordHash(this)
    ];
}
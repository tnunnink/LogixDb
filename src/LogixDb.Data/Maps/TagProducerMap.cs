using L5Sharp.Core;

namespace LogixDb.Data.Maps;

public class TagProducerMap : TableMap<ProduceInfo>
{
    /// <inheritdoc />
    protected override string TableName => "tag_producer";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ProduceInfo>> Columns =>
    [
        ColumnMap<ProduceInfo>.For(r => r.GetParent<Tag>()?.Metadata.Get<string>("record_hash"), "tag_hash", false),
        ColumnMap<ProduceInfo>.For(r => r.ProduceCount, "produce_count"),
        ColumnMap<ProduceInfo>.For(r => r.ProgrammaticallySendEventTrigger, "send_event_trigger"),
        ColumnMap<ProduceInfo>.For(r => r.UnicastPermitted, "unicast_permitted"),
        ColumnMap<ProduceInfo>.For(r => r.MaximumRPI, "maximum_rpi"),
        ColumnMap<ProduceInfo>.For(r => r.MinimumRPI, "minimum_rpi"),
        ColumnMap<ProduceInfo>.For(r => r.DefaultRPI, "default_rpi"),
        ColumnMap<ProduceInfo>.RecordHash(this)
    ];
}
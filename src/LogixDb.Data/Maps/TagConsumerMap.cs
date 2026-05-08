using L5Sharp.Core;

namespace LogixDb.Data.Maps;

internal class TagConsumerMap : TableMap<TagConsumeInfoRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_consumer";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagConsumeInfoRecord>> Columns =>
    [
        ColumnMap<TagConsumeInfoRecord>.For(r => r.TagId, "tag_id"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.Producer, "producer"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.RemoteTag, "remote_tag"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.RemoteInstance, "remote_instance"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.RPI, "rpi"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.Unicast, "unicast"),
        ColumnMap<TagConsumeInfoRecord>.For(r => r.ConsumeInfo.Hash(), "record_hash")
    ];
}

internal record TagConsumeInfoRecord(string? TagId, ConsumeInfo ConsumeInfo);
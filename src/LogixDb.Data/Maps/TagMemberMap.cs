using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

internal class TagMemberMap : TableMap<Tag>
{
    /// <inheritdoc />
    protected override string TableName => "tag_member";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Tag>> Columns =>
    [
        ColumnMap<Tag>.For(r => r.Base.Hash(), "tag_id"),
        ColumnMap<Tag>.For(r => r.Parent?.Hash(), "parent_id"), //todo this is not technically right
        ColumnMap<Tag>.For(r => r.TagName.LocalPath, "tag_name"),
        ColumnMap<Tag>.For(r => r.TagName.Element, "member_name"),
        ColumnMap<Tag>.For(r => r.GetDataTypeName(), "data_type"),
        ColumnMap<Tag>.For(r => r.Hash(), "record_hash")
        /*ColumnMap<TagMemberRecord>.For(r => r.Tag.Value.GetDataValue(), "tag_value")*/
    ];
}
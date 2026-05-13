using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

public class TagMemberMap : TableMap<Tag>
{
    /// <inheritdoc />
    protected override string TableName => "tag_member";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Tag>> Columns =>
    [
        ColumnMap<Tag>.For(r => r.Base.Metadata.Get<string>("record_hash"), "tag_hash"),
        ColumnMap<Tag>.For(r => r.TagName.LocalPath, "tag_name"),
        ColumnMap<Tag>.For(r => r.Parent?.TagName.LocalPath, "parent_name"),
        ColumnMap<Tag>.For(r => r.TagName.Element, "member_name"),
        ColumnMap<Tag>.For(r => r.GetDataTypeName(), "data_type")
    ];
}
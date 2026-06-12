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
        ColumnMap<Tag>.For(r => r.Base.Metadata.Get<string>("record_hash"), "tag_hash", hashable: false),
        ColumnMap<Tag>.For(r => r.TagName.MemberPath, "member_path"),
        ColumnMap<Tag>.For(r => r.TagName.MemberName, "member_name"),
        ColumnMap<Tag>.For(r => r.Parent?.TagName.MemberName, "parent_name"),
        ColumnMap<Tag>.For(r => r.GetDataTypeName(), "data_type")
    ];
}
using L5Sharp.Core;
using LogixDb.Core.Maps;

namespace LogixDb.Sqlite.Imports;

/// <summary>
/// Represents a class for importing tag data into a SQLite database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import tags into a SQLite database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqliteTagImport() : SqliteElementImport<Tag>(InsertTag, new TagMap())
{
    private const string InsertTag =
        """
        INSERT INTO tag (snapshot_id, reference, base_name, tag_name, scope_level, scope_name, tag_depth, tag_usage, data_type, value, description, dimensions, radix, external_access, opcua_access, constant, tag_type, alias_for, component_class, hash)
        VALUES (@snapshot_id, @reference, @base_name, @tag_name, @scope_level, @scope_name, @tag_depth, @tag_usage, @data_type, @value, @description, @dimensions, @radix, @external_access, @opcua_access, @constant, @tag_type, @alias_for, @component_class, @hash)
        """;

    /// <inheritdoc />
    protected override IEnumerable<Tag> GetRecords(L5X content)
    {
        return content.Query<Tag>().SelectMany(t => t.Members()).ToList();
    }
}
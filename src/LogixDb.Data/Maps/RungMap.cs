using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "rung" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Rung"/> class.
/// </summary>
public class RungMap : TableMap<Rung>
{
    /// <inheritdoc />
    protected override string TableName => "rung";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Rung>> Columns =>
    [
        ColumnMap<Rung>.For(r => r.Metadata.Get<Guid>("rung_id"), "rung_id"),
        ColumnMap<Rung>.For(r => r.Routine?.Metadata.Get<string>("record_hash"), "routine_hash", hashable: false),
        ColumnMap<Rung>.For(r => r.Number, "rung_number"),
        ColumnMap<Rung>.For(r => r.Text, "rung_text"),
        ColumnMap<Rung>.For(r => r.Comment, "rung_comment"),
        ColumnMap<Rung>.For(r => r.Text.HashText(), "code_hash"),
        ColumnMap<Rung>.RecordHash(this)
    ];
}
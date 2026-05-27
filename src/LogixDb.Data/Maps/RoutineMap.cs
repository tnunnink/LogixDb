using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "routine" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Routine"/> class.
/// </summary>
public class RoutineMap : TableMap<Routine>
{
    /// <inheritdoc />
    protected override string TableName => "routine";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Routine>> Columns =>
    [
        ColumnMap<Routine>.For(r => r.Reference.Scope.Container, "container_name"),
        ColumnMap<Routine>.For(r => r.Name, "routine_name"),
        ColumnMap<Routine>.For(r => r.Description, "routine_description"),
        ColumnMap<Routine>.For(r => r.Type.Name, "routine_type"),
        ColumnMap<Routine>.For(r => r.Reference.Scope.IsAoi, "is_definition"),
        ColumnMap<Routine>.For(r => r.HashElement(), "content_hash"),
        ColumnMap<Routine>.RecordHash(this)
    ];
}
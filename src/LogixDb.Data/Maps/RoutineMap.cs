using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "routine" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Routine"/> class.
/// </summary>
internal class RoutineMap : TableMap<Routine>
{
    /// <inheritdoc />
    protected override string TableName => "routine";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Routine>> Columns =>
    [
        ColumnMap<Routine>.For(r => r.Program?.Hash(), "program_id"),
        ColumnMap<Routine>.For(r => r.Name, "routine_name"),
        ColumnMap<Routine>.For(r => r.Type.Name, "routine_type"),
        ColumnMap<Routine>.For(r => r.Description, "routine_description"),
        ColumnMap<Routine>.For(r => r.Hash(), "record_hash"),
    ];
}
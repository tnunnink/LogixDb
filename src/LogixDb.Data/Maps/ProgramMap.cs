using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "program" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Program"/> class.
/// </summary>
internal class ProgramMap : TableMap<Program>
{
    /// <inheritdoc />
    protected override string TableName => "program";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Program>> Columns =>
    [
        ColumnMap<Program>.For(r => r.Task?.Hash(), "task_id"),
        ColumnMap<Program>.For(r => r.Parent?.Hash(), "folder_id"),
        ColumnMap<Program>.For(r => r.Name, "program_name"),
        ColumnMap<Program>.For(r => r.Description, "program_description"),
        ColumnMap<Program>.For(r => r.Type.Name, "program_type"),
        ColumnMap<Program>.For(r => r.MainRoutineName, "main_routine"),
        ColumnMap<Program>.For(r => r.FaultRoutineName, "fault_routine"),
        ColumnMap<Program>.For(r => r.Disabled, "is_disabled"),
        ColumnMap<Program>.For(r => r.UseAsFolder, "is_folder"),
        ColumnMap<Program>.For(r => r.TestEdits, "has_test_edits"),
        ColumnMap<Program>.For(r => r.Hash(), "record_hash")
    ];
}
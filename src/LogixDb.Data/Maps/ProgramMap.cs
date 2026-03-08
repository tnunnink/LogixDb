using L5Sharp.Core;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "program" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Program"/> class.
/// </summary>
public class ProgramMap : TableMap<ProgramRecord>
{
    /// <inheritdoc />
    public override string TableName => "program";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<ProgramRecord>> Columns =>
    [
        ColumnMap<ProgramRecord>.For(r => r.SnapshotId, "snapshot_id", false),
        ColumnMap<ProgramRecord>.For(r => r.Program.Name, "program_name"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Type.Name, "program_type"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Description, "description"),
        ColumnMap<ProgramRecord>.For(r => r.Program.MainRoutineName, "main_routine"),
        ColumnMap<ProgramRecord>.For(r => r.Program.FaultRoutineName, "fault_routine"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Disabled, "is_disabled"),
        ColumnMap<ProgramRecord>.For(r => r.Program.UseAsFolder, "is_folder"),
        ColumnMap<ProgramRecord>.For(r => r.Program.TestEdits, "has_test_edits"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Parent?.Name, "parent_name"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Task?.Name, "task_name"),
        ColumnMap<ProgramRecord>.For(ComputeHash, "record_hash", false)
    ];
}

/// <summary>
/// Represents a database record for a program entity.
/// This record contains the metadata for a specific Logix program,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this program record belongs.</param>
/// <param name="Program">The Logix program entity.</param>
public record ProgramRecord(int SnapshotId, Program Program);
using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "program" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Program"/> class.
/// </summary>
internal class ProgramMap : TableMap<ProgramRecord>
{
    /// <inheritdoc />
    protected override string TableName => "program";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ProgramRecord>> Columns =>
    [
        ColumnMap<ProgramRecord>.For(r => r.ProgramId, "program_id", hashable: false),
        ColumnMap<ProgramRecord>.For(r => r.InstanceId, "instance_id", hashable: false),
        ColumnMap<ProgramRecord>.For(r => r.TaskId, "task_id", hashable: false),
        ColumnMap<ProgramRecord>.For(r => r.FolderId, "folder_id", hashable: false),
        ColumnMap<ProgramRecord>.For(r => r.Program.Name, "program_name", hashable: false),
        ColumnMap<ProgramRecord>.For(r => r.Program.Description, "program_description"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Type.Name, "program_type"),
        ColumnMap<ProgramRecord>.For(r => r.Program.MainRoutineName, "main_routine"),
        ColumnMap<ProgramRecord>.For(r => r.Program.FaultRoutineName, "fault_routine"),
        ColumnMap<ProgramRecord>.For(r => r.Program.Disabled, "is_disabled"),
        ColumnMap<ProgramRecord>.For(r => r.Program.UseAsFolder, "is_folder"),
        ColumnMap<ProgramRecord>.For(r => r.Program.TestEdits, "has_test_edits"),
        ColumnMap<ProgramRecord>.For(ComputeHash, "record_hash", hashable: false),
        ColumnMap<ProgramRecord>.For(r => r.Program.Hash(), "source_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a program entity.
/// This record contains the metadata for a specific Logix program,
/// as well as the unique identifier linking it to a specific database target.
/// </summary>
internal record ProgramRecord(int InstanceId, Guid? TaskId, Guid? FolderId, Program Program)
{
    public Guid ProgramId { get; } = Guid.NewGuid();
}

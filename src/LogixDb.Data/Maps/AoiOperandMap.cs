using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a table mapping for AOI (Add-On Instruction) operands, which defines how operand data
/// is extracted from Logix snapshots and mapped to database records.
/// </summary>
public class AoiOperandMap : TableMap<AoiOperandRecord>
{
    /// <inheritdoc />
    public override string TableName => "operand";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<AoiOperandRecord>> Columns =>
    [
        ColumnMap<AoiOperandRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<AoiOperandRecord>.For(r => r.Key, "instruction_key"),
        ColumnMap<AoiOperandRecord>.For(r => r.Index, "operand_index"),
        ColumnMap<AoiOperandRecord>.For(r => r.Name, "operand_name"),
        ColumnMap<AoiOperandRecord>.For(r => r.Type, "operand_type"),
        ColumnMap<AoiOperandRecord>.For(r => r.Description, "operand_description"),
        ColumnMap<AoiOperandRecord>.For(r => r.Destructive, "is_destructive")
    ];

    /// <inheritdoc />
    public override IEnumerable<AoiOperandRecord> GetRecords(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = new List<AoiOperandRecord>();

        var instructions = source.Query<AddOnInstruction>().ToList();

        foreach (var instruction in instructions)
        {
            byte index = 0;

            records.Add(new AoiOperandRecord(
                snapshot.SnapshotId,
                instruction.Name,
                index,
                instruction.Name,
                instruction.Name,
                instruction.Description,
                true
            ));

            foreach (var parameter in instruction.Parameters.Where(p => p.Required is true))
            {
                index++;

                var operand = new AoiOperandRecord(
                    snapshot.SnapshotId,
                    instruction.Name,
                    index,
                    parameter.Name,
                    parameter.DataType,
                    parameter.Description,
                    parameter.Usage == TagUsage.InOut || parameter.Usage == TagUsage.Output
                );

                records.Add(operand);
            }
        }

        return records;
    }
}

/// <summary>
/// Represents a record that defines the structure and properties of an operand used in an AOI (Add-On Instruction).
/// This record is part of the LogixDB data mapping system and corresponds to entries in the "operand" table.
/// </summary>
/// <remarks>
/// The AoiOperandRecord is a data structure designed to hold information about individual operands associated
/// with AOI instructions. Each operand has properties such as its unique key, type, format, and other metadata
/// that describe its functionality and behavior in the context of AOI usage.
/// </remarks>
public record AoiOperandRecord(
    int SnapshotId,
    string Key,
    byte Index,
    string Name,
    string Type,
    string? Description,
    bool Destructive
);
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class responsible for importing AOI (Add-On Instruction) operands
/// from a given L5X content structure into a SQL Server database.
/// </summary>
internal class SqlServerAoiOperandImport : SqlServerImport
{
    private readonly AoiOperandMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = new List<AoiOperandRecord>();

        foreach (var instruction in source.AddOnInstructions)
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

        return ImportRecords(records, _map, session, token);
    }
}
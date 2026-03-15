using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class responsible for importing AOI (Add-On Instruction) parameters
/// from a given L5X content structure into a SQL Server database.
/// </summary>
internal class SqlServerAoiParameterImport : SqlServerImport
{
    private readonly AoiParameterMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.AddOnInstructions.SelectMany(instruction =>
            instruction.Parameters.Select(parameter =>
                new AoiParameterRecord(snapshot.SnapshotId, instruction.Name, parameter)));
        return ImportRecords(records, _map, session, token);
    }
}
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Provides functionality to import Add-On Instruction (AOI) elements into a SQL Server database.
/// This class is responsible for retrieving AOI records from an L5X content source and mapping them to a database structure for import.
/// </summary>
internal class SqlServerAoiImport : SqlServerImport
{
    private readonly AoiMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.AddOnInstructions.Select(a => new AoiRecord(snapshot.SnapshotId, a));
        return ImportRecords(records, _map, session, token);
    }
}
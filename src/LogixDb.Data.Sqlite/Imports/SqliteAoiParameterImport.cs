using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of Add-On Instruction (AOI) parameter records from a LogixDb snapshot into an SQLite database.
/// This class processes AOI parameters by querying them from the snapshot source and inserting
/// them into the database using the configured AOI parameter table mapping.
/// </summary>
internal class SqliteAoiParameterImport : SqliteImport
{
    private readonly AoiParameterMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        var records = source.AddOnInstructions
            .SelectMany(a => a.Parameters.Select(p => new AoiParameterRecord(snapshot.SnapshotId, a.Name, p)))
            .ToList();
        await ImportRecords(records, _map, command, token);
    }
}
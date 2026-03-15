using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of Add-On Instruction (AOI) records from a LogixDb snapshot into an SQLite database.
/// This class processes AOI entities by querying them from the snapshot source and inserting
/// them into the database using the configured AOI table mapping.
/// </summary>
internal class SqliteAoiImport : SqliteImport
{
    private readonly AoiMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        var records = source.AddOnInstructions.Select(a => new AoiRecord(snapshot.SnapshotId, a)).ToList();
        await ImportRecords(records, _map, command, token);
    }
}
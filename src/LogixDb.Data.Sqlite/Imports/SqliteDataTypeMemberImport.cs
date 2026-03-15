using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of data type member records from a LogixDb snapshot into an SQLite database.
/// This class processes data type member entities by querying them from the snapshot source and inserting
/// them into the database using the configured data type member table mapping.
/// </summary>
internal class SqliteDataTypeMemberImport : SqliteImport
{
    private readonly DataTypeMemberMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        
        var records = source.Query<DataType>()
            .SelectMany(d => d.Members)
            .Select(m => new DataTypeMemberRecord(snapshot.SnapshotId, m))
            .ToList();
        
        await ImportRecords(records, _map, command, token);
    }
}
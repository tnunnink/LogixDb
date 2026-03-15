using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of data type records from a LogixDb snapshot into an SQLite database.
/// This class processes data type entities by querying them from the snapshot source and inserting
/// them into the database using the configured data type table mapping.
/// </summary>
internal class SqliteDataTypeImport : SqliteImport
{
    private readonly DataTypeMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        var records = source.Query<DataType>().Select(d => new DataTypeRecord(snapshot.SnapshotId, d)).ToList();
        await ImportRecords(records, _map, command, token);
    }
}
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of program records from a LogixDb snapshot into an SQLite database.
/// This class processes program entities by querying them from the snapshot source and inserting
/// them into the database using the configured program table mapping.
/// </summary>
internal class SqliteProgramImport : SqliteImport
{
    private readonly ProgramMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        var records = source.Query<Program>().Select(p => new ProgramRecord(snapshot.SnapshotId, p)).ToList();
        await ImportRecords(records, _map, command, token);
    }
}
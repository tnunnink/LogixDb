using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of controller records from a LogixDb snapshot into an SQLite database.
/// This class processes the controller entity by querying it from the snapshot source and inserting
/// it into the database using the configured controller table mapping.
/// </summary>
internal class SqliteControllerImport : SqliteImport
{
    private readonly ControllerMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = new List<ControllerRecord> { new(snapshot.SnapshotId, source.Controller) };
        await ImportRecords(records, _map, session, token);
    }
}
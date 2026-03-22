using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of module records from a LogixDb snapshot into an SQLite database.
/// This class processes module entities by querying them from the snapshot source and inserting
/// them into the database using the configured module table mapping.
/// </summary>
internal class SqliteModuleImport : SqliteImport
{
    private readonly ModuleMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();

        //Some module elements don't have a name, and we need to skip that for now...
        var records = source.Modules
            .Where(m => !string.IsNullOrEmpty(m.Name))
            .Select(m => new ModuleRecord(snapshot.SnapshotId, m));

        await ImportRecords(records, _map, session, token);
    }
}
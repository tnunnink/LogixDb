using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing module data into a SqlServer database.
/// </summary>
internal class SqlServerModuleImport : SqlServerImport
{
    private readonly ModuleMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        
        //Some module elements don't have a name, and we need to skip that for now...
        var records = source.Modules
            .Where(m => !string.IsNullOrEmpty(m.Name))
            .Select(m => new ModuleRecord(snapshot.SnapshotId, m));
        
        return ImportRecords(records, _map, session, token);
    }
}
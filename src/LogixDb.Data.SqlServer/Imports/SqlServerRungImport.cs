using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing rung data into a SqlServer database.
/// </summary>
internal class SqlServerRungImport : SqlServerImport
{
    private readonly RungMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.Query<Rung>().Select(r => new RungRecord(snapshot.SnapshotId, r));
        return ImportRecords(records, _map, session, token);
    }
}
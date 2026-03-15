using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// A class responsible for importing controller data from an L5X file into an SqlServer database.
/// </summary>
internal class SqlServerControllerImport : SqlServerImport
{
    private readonly ControllerMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = new[] { new ControllerRecord(snapshot.SnapshotId, source.Controller) };
        return ImportRecords(records, _map, session, token);
    }
}
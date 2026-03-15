using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing program data into a SqlServer database.
/// </summary>
internal class SqlServerProgramImport : SqlServerImport
{
    private readonly ProgramMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.Programs.Select(p => new ProgramRecord(snapshot.SnapshotId, p));
        return ImportRecords(records, _map, session, token);
    }
}
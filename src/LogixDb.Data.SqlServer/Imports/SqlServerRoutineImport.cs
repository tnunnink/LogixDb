using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing routine data into a SqlServer database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import routines into a SqlServer database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqlServerRoutineImport : SqlServerImport
{
    private readonly RoutineMap _map = new();

    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.Query<Routine>().Select(r => new RoutineRecord(snapshot.SnapshotId, r));
        return ImportRecords(records, _map, session, token);
    }
}
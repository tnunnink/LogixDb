using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing data type member data into a SqlServer database.
/// </summary>
internal class SqlServerDataTypeMemberImport : SqlServerImport
{
    private readonly DataTypeMemberMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.DataTypes.SelectMany(d =>
            d.Members.Select(m => new DataTypeMemberRecord(snapshot.SnapshotId, m)));
        return ImportRecords(records, _map, session, token);
    }
}
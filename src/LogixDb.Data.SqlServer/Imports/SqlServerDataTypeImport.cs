using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing data type data into a SqlServer database.
/// </summary>
internal class SqlServerDataTypeImport : SqlServerImport
{
    private readonly DataTypeMap _map = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.DataTypes.Select(d => new DataTypeRecord(snapshot.SnapshotId, d));
        return ImportRecords(records, _map, session, token);
    }
}
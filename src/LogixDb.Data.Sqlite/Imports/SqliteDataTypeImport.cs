using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of data type records from a LogixDb snapshot into an SQLite database.
/// This class processes data type entities by querying them from the snapshot source and inserting
/// them into the database using the configured data type table mapping.
/// </summary>
internal class SqliteDataTypeImport : SqliteImport
{
    private readonly DataTypeMap _dataTypeMap = new();
    private readonly DataTypeMemberMap _memberMap = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var dataTypeRecords = new List<DataTypeRecord>();
        var memberRecords = new List<DataTypeMemberRecord>();

        foreach (var dataType in source.DataTypes.Where(d => d.Class == DataTypeClass.User))
        {
            dataTypeRecords.Add(new DataTypeRecord(snapshot.SnapshotId, dataType));
            memberRecords.AddRange(dataType.Members.Select(m => new DataTypeMemberRecord(snapshot.SnapshotId, m)));
        }

        await ImportRecords(dataTypeRecords, _dataTypeMap, session, token);
        await ImportRecords(memberRecords, _memberMap, session, token);
    }
}
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
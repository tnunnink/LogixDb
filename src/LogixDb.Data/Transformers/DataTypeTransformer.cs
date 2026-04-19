using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on user-defined data types and their members.
/// </summary>
/// <remarks>
/// The <c>DataTypeTransformer</c> identifies user-defined data types within a given
/// snapshot and converts them into a tabular structure suitable for database persistence.
/// The transformation process involves creating records for the data types and their
/// associated members, which are subsequently mapped into tables.
/// </remarks>
internal class DataTypeTransformer : ISnapshotTransformer
{
    private readonly DataTypeMap _typeMap = new();
    private readonly DataTypeMemberMap _memberMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var dataTypeRecords = new List<DataTypeRecord>();
        var memberRecords = new List<DataTypeMemberRecord>();

        foreach (var dataType in source.DataTypes.Where(d => d.Class == DataTypeClass.User))
        {
            var type = new DataTypeRecord(snapshot.SnapshotId, dataType);
            var members = dataType.Members.Select(m => new DataTypeMemberRecord(snapshot.SnapshotId, type.TypeId, m));

            dataTypeRecords.Add(type);
            memberRecords.AddRange(members);
        }

        yield return _typeMap.GenerateTable(dataTypeRecords);
        yield return _memberMap.GenerateTable(memberRecords);
    }
}
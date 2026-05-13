using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on user-defined data types and their members.
/// </summary>
/// <remarks>
/// The <c>DataTypeTransformer</c> identifies user-defined data types within a given
/// Target and converts them into a tabular structure suitable for database persistence.
/// The transformation process involves creating records for the data types and their
/// associated members, which are subsequently mapped into tables.
/// </remarks>
public class DataTypeTransformer : IDbTransformer
{
    private readonly DataTypeMap _typeMap = new();
    private readonly DataTypeMemberMap _memberMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        yield return _typeMap.GenerateTable(source.DataTypes);
        yield return _memberMap.GenerateTable(source.DataTypes.SelectMany(d => d.Members));
    }
}
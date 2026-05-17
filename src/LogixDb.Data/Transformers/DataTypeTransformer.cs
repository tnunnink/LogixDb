using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

public class DataTypeTransformer : IDbTransformer
{
    private readonly DataTypeMap _typeMap = new();
    private readonly DataTypeMemberMap _memberMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var types = new List<DataType>();
        var members = new List<DataTypeMember>();

        foreach (var dataType in source.DataTypes)
        {
            // Predefined will be seeded with each database upon migration.
            if (dataType.Class == DataTypeClass.Predefined) continue;

            types.Add(dataType);

            var index = 0;
            foreach (var member in dataType.Members)
            {
                // We will only want public, non-hidden members to show next valid index number.
                if (!member.Hidden)
                {
                    member.Metadata["member_index"] = index;
                    index++;
                }
                else
                {
                    member.Metadata["member_index"] = -1;
                }

                members.Add(member);
            }
        }

        yield return _typeMap.GenerateTable(types);
        yield return _memberMap.GenerateTable(members);
    }
}
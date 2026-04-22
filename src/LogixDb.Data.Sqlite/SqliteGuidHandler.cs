using System.Data;
using Dapper;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// A custom Dapper type handler for handling the mapping of <see cref="Guid"/> values to and from SQLite database fields.
/// </summary>
/// <remarks>
/// This class ensures that <see cref="Guid"/> values can be properly stored and retrieved from SQLite databases,
/// which typically do not natively support <see cref="Guid"/> types.
/// </remarks>
internal class SqliteGuidHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid guid)
    {
        if (guid == Guid.Empty)
        {
            parameter.Value = null;
            return;
        }
        
        parameter.Value = guid.ToString();
    }

    public override Guid Parse(object? value)
    {
        return value is null or DBNull ? Guid.Empty : Guid.Parse((string)value);
    }
}
using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Provides functionality to import Add-On Instruction (AOI) elements into a SQL Server database.
/// This class is responsible for retrieving AOI records from an L5X content source and mapping them to a database structure for import.
/// Derived from <see cref="SqlServerImport{TElement}"/>, where TElement is <see cref="AddOnInstruction"/>.
/// </summary>
internal class SqlServerAoiImport() : SqlServerImport<AoiRecord>(new AoiMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        var records = source.Query<AddOnInstruction>()
            .Select(x => new AoiRecord(snapshot.SnapshotId, x))
            .ToList();

        return Map.GenerateTable(records);
    }
}
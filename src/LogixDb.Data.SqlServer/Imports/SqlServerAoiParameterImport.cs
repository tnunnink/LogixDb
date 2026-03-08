using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class responsible for importing AOI (Add-On Instruction) parameters
/// from a given L5X content structure into a SQL Server database. This class extends
/// the base <see cref="SqlServerImport{TElement}"/> for handling the import process
/// specific to AOI parameters.
/// </summary>
/// <remarks>
/// The import process retrieves AOI parameter elements from the L5X content
/// and maps them to the corresponding database table using the <see cref="AoiParameterMap"/>.
/// </remarks>
/// <seealso cref="SqlServerImport{TElement}"/>
internal class SqlServerAoiParameterImport() : SqlServerImport<AoiParameterRecord>(new AoiParameterMap())
{
    /// <inheritdoc />
    protected override IEnumerable<AoiParameterRecord> GetRecords(Snapshot snapshot)
    {
        return snapshot.GetSource().Query<AddOnInstruction>()
            .SelectMany(aoi => aoi.Parameters)
            .Select(x => new AoiParameterRecord(snapshot.SnapshotId, x))
            .ToList();
    }
}

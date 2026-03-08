using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// A class responsible for importing controller data from an L5X file into an SqlServer database.
/// Implements element import for <see cref="Controller"/> objects by extracting the single
/// controller instance from the L5X content and mapping it to the database using <see cref="ControllerMap"/>.
/// </summary>
internal class SqlServerControllerImport() : SqlServerImport<ControllerRecord>(new ControllerMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        List<ControllerRecord> records = [new(snapshot.SnapshotId, source.Controller)];
        return Map.GenerateTable(records);
    }
}
using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents the mapping configuration for the "aoi_rung" database table.
/// Provides metadata about the table name and its associated column mappings.
/// This class is used to define how the data is structured and translated into the database.
/// </summary>
public class AoiRungMap : TableMap<AoiRungRecord>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_rung";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiRungRecord>> Columns =>
    [
        ColumnMap<AoiRungRecord>.For(r => r.AoiName, "aoi_name"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Routine?.Name, "routine_name"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Number, "rung_number"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Comment, "rung_comment"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Text, "rung_text"),
        ColumnMap<AoiRungRecord>.RecordHash(this)
    ];
}

public record AoiRungRecord(string? AoiName, Rung Rung);
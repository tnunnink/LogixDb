using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

public class AoiRungMap : TableMap<AoiRungRecord>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_rung";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiRungRecord>> Columns =>
    [
        ColumnMap<AoiRungRecord>.For(r => r.AoiHash, "aoi_hash"),
        ColumnMap<AoiRungRecord>.For(r => r.RoutineName, "routine_name"),
        ColumnMap<AoiRungRecord>.For(r => r.Number, "rung_number"),
        ColumnMap<AoiRungRecord>.For(r => r.Comment, "rung_comment"),
        ColumnMap<AoiRungRecord>.For(r => r.Text, "rung_text"),
        ColumnMap<AoiRungRecord>.RecordHash(this)
    ];
}

public record AoiRungRecord(string AoiHash, string? RoutineName, int Number, string? Comment, string Text)
{
    public static AoiRungRecord FromRung(Rung rung, string aoiHash)
    {
        return new AoiRungRecord(aoiHash, rung.Routine?.Name, rung.Number, rung.Comment, rung.Text);
    }
}
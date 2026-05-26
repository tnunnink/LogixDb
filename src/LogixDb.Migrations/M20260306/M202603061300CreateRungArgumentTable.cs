using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061300, "Creates argument table with corresponding indexes and foreign key relationships")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061300CreateArgumentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung_argument")
            .WithRelation<Guid>("rung_id", "rung").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("instruction_index").AsInt16().NotNullable()
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("argument_type").AsString(32).NotNullable()
            .WithColumn("argument_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("rung_argument")
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .OnColumn("argument_index").Ascending();
    }
}
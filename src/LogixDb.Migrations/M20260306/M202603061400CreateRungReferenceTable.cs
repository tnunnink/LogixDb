using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061400, "Creates rung_reference table with indexes for reference name lookups")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061400CreateRungReferenceTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung_reference").InLogixSchema()
            .WithRelation<long>("rung_id", "rung").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("instruction_index").AsInt16().NotNullable()
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("reference_name").AsString(256).NotNullable();

        Create.Index()
            .OnTable("rung_reference").InLogixSchema()
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .OnColumn("argument_index").Ascending()
            .WithOptions().Clustered();

        Create.Index()
            .OnTable("rung_reference").InLogixSchema()
            .OnColumn("reference_name").Ascending()
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .OnColumn("argument_index").Ascending();
    }
}
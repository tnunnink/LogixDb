using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061400, "Creates code_reference table with indexes for reference name lookups")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061400CreateCodeReferenceTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("code_reference")
            .WithRelation<Guid>("rung_id", "rung").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("instruction_index").AsInt16().NotNullable()
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("reference_name").AsString(256).NotNullable();

        Create.Index().OnTable("code_reference")
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .OnColumn("argument_index").Ascending();

        Create.Index().OnTable("code_reference")
            .OnColumn("reference_name").Ascending()
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .OnColumn("argument_index").Ascending();
    }
}
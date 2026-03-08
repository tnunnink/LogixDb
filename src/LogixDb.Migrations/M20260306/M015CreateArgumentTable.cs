using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260306;

[PublicAPI]
[Migration(202603061300, "")]
public class M015CreateArgumentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("argument")
            .WithPrimaryId("argument_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("instruction_hash").AsBinary(16).NotNullable()
            .WithColumn("argument_ordinal").AsByte().NotNullable()
            .WithColumn("argument_type").AsString(32).NotNullable()
            .WithColumn("argument_text").AsString(255).NotNullable()
            .WithColumn("argument_tags").AsString(2048).Nullable()
            .WithColumn("argument_values").AsString(2048).Nullable()
            .WithColumn("record_hash").AsBinary(16).NotNullable();

        Create.Index()
            .OnTable("argument")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("instruction_hash").Ascending();
        
        Create.Index()
            .OnTable("argument")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}
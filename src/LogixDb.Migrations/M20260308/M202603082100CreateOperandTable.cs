using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260308;

[UsedImplicitly]
[Migration(202603082100, "Creates operand table with unique composite index on instruction_key and operand_index")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic, MigrationTag.Aoi)]
public class M202603082100CreateOperandTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("operand")
            .WithPrimaryKey<long>("operand_id")
            .WithColumn("instruction_key").AsString(128).NotNullable()
            .WithColumn("operand_index").AsByte().NotNullable()
            .WithColumn("operand_name").AsString(128).NotNullable()
            .WithColumn("operand_type").AsString(128).Nullable()
            /*.WithColumn("operand_format").AsString(32).Nullable()*/
            .WithColumn("operand_description").AsString(2000).Nullable()
            .WithColumn("is_destructive").AsBoolean().NotNullable()
            .WithColumn("is_native").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("operand")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("operand")
            .OnColumn("instruction_key").Ascending()
            .OnColumn("operand_index").Ascending();
    }
}
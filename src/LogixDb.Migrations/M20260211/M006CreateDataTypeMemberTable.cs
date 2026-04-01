using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111530, "Creates data_type_member table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M006CreateDataTypeMemberTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("data_type_member")
            .WithPrimaryGuid("member_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("type_name").AsString(128).NotNullable()
            .WithColumn("member_name").AsString(128).NotNullable()
            .WithColumn("data_type").AsString(128).Nullable()
            .WithColumn("dimension").AsInt16().Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("member_description").AsString(512).Nullable()
            .WithColumn("is_hidden").AsBoolean().Nullable()
            .WithColumn("target_name").AsString(64).Nullable()
            .WithColumn("bit_number").AsByte().Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("data_type_member")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("type_name").Ascending()
            .OnColumn("member_name").Ascending()
            .WithOptions().Unique();
    }
}
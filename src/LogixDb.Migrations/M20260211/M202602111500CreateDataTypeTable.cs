using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111500, "Creates data_type table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M202602111500CreateDataTypeTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("data_type")
            .WithPrimaryGuid("type_id")
            .WithSnapshotRelation(nullable: true)
            .WithColumn("type_name").AsString(256).NotNullable()
            .WithColumn("type_description").AsString(512).Nullable()
            .WithColumn("type_class").AsString(32).Nullable()
            .WithColumn("type_family").AsString(32).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable()
            .WithColumn("source_hash").AsString(32).NotNullable();

        Create.Index().OnTable("data_type")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("type_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("data_type")
            .OnColumn("type_name").Ascending()
            .OnColumn("record_hash").Ascending();

        Create.Index().OnTable("data_type")
            .OnColumn("source_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}
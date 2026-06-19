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
        Create.Table("data_type").InLogixSchema()
            .WithPrimaryKey<long>("type_id")
            .WithColumn("type_name").AsString(256).NotNullable()
            .WithColumn("type_description").AsString(512).Nullable()
            .WithColumn("type_class").AsString(32).Nullable()
            .WithColumn("type_family").AsString(32).Nullable()
            .WithColumn("content_hash").AsString(64).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index()
            .OnTable("data_type").InLogixSchema()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("data_type").InLogixSchema()
            .OnColumn("type_name").Ascending();
    }
}
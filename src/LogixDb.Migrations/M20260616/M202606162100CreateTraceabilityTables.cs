using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260616;

[UsedImplicitly]
[Migration(202606162100, "Creates import traceability tables for ingestion tracking")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202606162100CreateTraceabilityTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("import")
            .WithPrimaryKey<Guid>("import_id")
            .WithColumn("import_status").AsString(64).NotNullable()
            .WithColumn("source_type").AsString(64).NotNullable()
            .WithColumn("file_type").AsString(256).NotNullable()
            .WithColumn("file_name").AsString(256).NotNullable()
            .WithColumn("posted_on").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.Index().OnTable("import")
            .OnColumn("file_name").Ascending();

        Create.Index().OnTable("import")
            .OnColumn("posted_on").Descending();

        Create.Table("import_log")
            .WithPrimaryKey<long>("log_id")
            .WithRelation<Guid>("import_id", "import").OnDelete(System.Data.Rule.Cascade).NotNullable()
            .WithColumn("timestamp").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("log_severity").AsString(64).NotNullable()
            .WithColumn("log_message").AsString(int.MaxValue).NotNullable()
            .WithColumn("log_exception").AsString(int.MaxValue);

        Create.Index().OnTable("import_log")
            .OnColumn("import_id").Ascending();
    }
}
using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606120900, "Create QA schema and basic user defined types")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606120900CreateQaSchemaAndTypes : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.001_QaSchemaAndTypes.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP TYPE IF EXISTS [qa].[variables]");
        Execute.Sql("DROP TYPE IF EXISTS [qa].[validations]");
        Execute.Sql("DROP TYPE IF EXISTS [qa].[outcome]");
        Execute.Sql("DROP SCHEMA IF EXISTS [qa]");
    }
}


using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121130, "Create QA inspect result function")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121130CreateQaInspectResultFunction : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.QA.QA.QaInspectResultFunction.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS [qa].[inspect_result]");
    }
}


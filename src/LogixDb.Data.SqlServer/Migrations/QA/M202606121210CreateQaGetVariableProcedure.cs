using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121210, "Create QA get variable procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121210CreateQaGetVariableProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.QA.QA.QaGetVariableProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[get_variable]");
    }
}


using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121220, "Create QA get variable as int procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121220CreateQaGetVariableAsIntProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.QA.QA.QaGetVariableAsIntProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[get_variable_as_int]");
    }
}


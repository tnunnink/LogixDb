using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606161323, "Create QA get variable as real procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606161323CreateQaGetVariableAsRealProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.014_QaGetVariableAsRealProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[get_variable_as_real]");
    }
}

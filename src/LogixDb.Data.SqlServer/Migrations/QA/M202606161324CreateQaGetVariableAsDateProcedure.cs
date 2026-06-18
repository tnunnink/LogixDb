using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606161324, "Create QA get variable as date procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606161324CreateQaGetVariableAsDateProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.015_QaGetVariableAsDateProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[get_variable_as_date]");
    }
}

using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606161322, "Create QA get variable as bit procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606161322CreateQaGetVariableAsBitProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.013_QaGetVariableAsBitProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[get_variable_as_bit]");
    }
}

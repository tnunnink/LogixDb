using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606161327, "Create QA rerun validations procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606161327CreateQaRerunValidationsProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.016_QaRerunValidationsProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[rerun_validations]");
    }
}

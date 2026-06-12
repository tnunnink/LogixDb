using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121240, "Create QA run validations procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121240CreateQaRunValidationsProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.QA.QA.QaRunValidationsProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[run_validations]");
    }
}


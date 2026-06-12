using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121230, "Create QA run validation procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121230CreateQaRunValidationProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.QA.QA.QaRunValidationProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[run_validation]");
    }
}


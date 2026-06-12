using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121200, "Create QA create validation procedure")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121200CreateQaCreateValidationProcedure : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.008_QaCreateValidationProcedure.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS [qa].[create_validation]");
    }
}


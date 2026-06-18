using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121110, "Create QA emit failure function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606121110CreateQaEmitFailureFunction : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.005_QaEmitFailureFunction.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS [qa].[emit_failure]");
    }
}


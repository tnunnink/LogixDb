using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121120, "Create QA emit success function")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121120CreateQaEmitSuccessFunction : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.006_QaEmitSuccessFunction.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS [qa].[emit_success]");
    }
}


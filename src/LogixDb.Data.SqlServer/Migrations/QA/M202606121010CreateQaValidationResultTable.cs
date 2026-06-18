using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121010, "Create QA validation result table")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606121010CreateQaValidationResultTable : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.003_QaValidationResultTable.sql");
    }

    public override void Down()
    {
        Delete.Table("validation_result").InSchema("qa");
    }
}


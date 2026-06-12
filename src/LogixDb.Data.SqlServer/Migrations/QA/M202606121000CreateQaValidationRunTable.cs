using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121000, "Create QA validation run table")]
[Tags(TagBehavior.RequireAny, MigrationTag.QA)]
public class M202606121000CreateQaValidationRunTable : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.002_QaValidationRunTable.sql");
    }

    public override void Down()
    {
        Delete.Table("validation_run").InSchema("qa");
    }
}


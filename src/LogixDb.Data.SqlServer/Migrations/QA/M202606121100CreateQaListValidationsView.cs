using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.QA;

[UsedImplicitly]
[Migration(202606121100, "Create QA list validations view")]
[Tags(TagBehavior.RequireAny, MigrationTag.Qa)]
public class M202606121100CreateQaListValidationsView : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("QA.004_QaListValidationsView.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP VIEW IF EXISTS [qa].[list_validations]");
    }
}


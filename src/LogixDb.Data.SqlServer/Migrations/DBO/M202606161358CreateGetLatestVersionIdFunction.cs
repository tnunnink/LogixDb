using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202606161358, "Create get_latest_version_id function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202606161358CreateGetLatestVersionIdFunction : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("DBO.GetLatestVersionId.sql");
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS [dbo].[get_latest_version_id]");
    }
}

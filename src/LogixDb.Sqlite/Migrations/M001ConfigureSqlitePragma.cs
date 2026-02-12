using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Sqlite.Migrations;

[UsedImplicitly]
//[Migration(202602110600, "Apply Sqlite specific PRAGMA settings for performance enhancements")]
public class M001ConfigureSqlitePragma : Migration
{
    public override void Up()
    {
        // Set journal mode for better concurrency
        Execute.Sql("PRAGMA journal_mode = WAL");
        
        // Set synchronous mode for better performance
        Execute.Sql("PRAGMA synchronous = NORMAL");
        
        // 
        Execute.Sql("PRAGMA busy_timeout = 5000;");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
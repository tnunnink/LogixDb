using NUnit.Framework.Legacy;

namespace LogixDb.Sqlite.Tests;

[TestFixture]
public class SqliteMigrationTest() : SqliteMigrationTestBase("../../../logix.db")
{
    /// <summary>
    /// This test is mostly just to refresh a local db instance to inspect and write queries against.
    /// </summary>
    [Test]
    public void Migrate_WhenCalled_ShouldUpdateDatabase()
    {
        MigrateUp();
        
        FileAssert.Exists("../../../logix.db");
    }
}
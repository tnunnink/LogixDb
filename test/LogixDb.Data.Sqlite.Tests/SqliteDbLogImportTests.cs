using LogixDb.Testing;

namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbLogImportTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task LogImport_ValidLog_InsertsRecord()
    {
        var import = Import.Create("test.L5X", "C:\\Temp", SourceType.CLI);
        await Database.PutImport(import);
        var log = import.Info("Test message");

        await Database.LogImport(log);

        await AssertRecordExists("import_log", "log_message", "Test message");
    }

    [Test]
    public async Task LogImport_MultipleLogs_InsertsMultipleRecords()
    {
        var import = Import.Create("test.L5X", "C:\\Temp", SourceType.CLI);
        await Database.PutImport(import);
        
        await Database.LogImport(import.Info("Message 1"));
        await Database.LogImport(import.Info("Message 2"));

        await AssertRecordCount("import_log", 2);
    }
}

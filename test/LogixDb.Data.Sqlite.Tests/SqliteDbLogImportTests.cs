namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbLogImportTests : SqliteTestFixture
{
    [Test]
    public async Task LogImport_ValidLog_InsertsRecord()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);
        await Manager.PutImport(import);
        var log = import.Info("Test message");

        await Manager.LogImport(log);

        await AssertRecordExists("import_log", "log_message", "Test message");
    }

    [Test]
    public async Task LogImport_MultipleLogs_InsertsMultipleRecords()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);
        await Manager.PutImport(import);

        await Manager.LogImport(import.Info("Message 1"));
        await Manager.LogImport(import.Info("Message 2"));

        await AssertRecordCount("import_log", 2);
    }
}
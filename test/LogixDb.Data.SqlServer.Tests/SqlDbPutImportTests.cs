namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbPutImportTests : SqlServerTestFixture
{
    [Test]
    public async Task PutImport_NewImport_InsertsRecord()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);

        await Manager.CreateImport(import);

        await AssertRecordExists("logix.import", "file_name", "test");
        await AssertRecordExists("logix.import", "file_type", "L5X");
        await AssertRecordExists("logix.import", "source_type", "CLI");
    }

    [Test]
    public async Task PutImport_ExistingImport_UpdatesStatus()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);
        await Manager.CreateImport(import);

        import.Status = ImportStatus.Complete;
        await Manager.CreateImport(import);

        await AssertRecordExists("logix.import", "import_status", nameof(ImportStatus.Complete));
    }
}
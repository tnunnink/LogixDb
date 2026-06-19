namespace LogixDb.Data.Sqlite.Tests;

[TestFixture]
public class SqliteDbPutImportTests : SqliteTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task PutImport_NewImport_InsertsRecord()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);

        await Database.PutImport(import);

        await AssertRecordExists("import", "file_name", "test");
        await AssertRecordExists("import", "file_type", "L5X");
        await AssertRecordExists("import", "source_type", "CLI");
    }

    [Test]
    public async Task PutImport_ExistingImport_UpdatesStatus()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);
        await Database.PutImport(import);

        import.Status = ImportStatus.Complete;
        await Database.PutImport(import);

        await AssertRecordExists("import", "import_status", nameof(ImportStatus.Complete));
    }
}
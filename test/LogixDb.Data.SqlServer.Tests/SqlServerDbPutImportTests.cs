namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlServerDbPutImportTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Database.Migrate();
    }

    [Test]
    public async Task PutImport_NewImport_InsertsRecord()
    {
        var import = Import.Create("test.L5X", "C:\\Temp", SourceType.CLI);

        await Database.PutImport(import);

        await AssertRecordExists("import", "file_name", "test.L5X");
    }

    [Test]
    public async Task PutImport_ExistingImport_UpdatesStatus()
    {
        var import = Import.Create("test.L5X", "C:\\Temp", SourceType.CLI);
        await Database.PutImport(import);

        import.ImportStatus = ImportStatus.Complete;
        await Database.PutImport(import);

        await AssertRecordExists("import", "import_status", nameof(ImportStatus.Complete));
    }
}
namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbPutImportTests : SqlServerTestFixture
{
    [SetUp]
    protected async Task Setup()
    {
        await Migrator.Migrate(Connection);
    }

    [Test]
    public async Task PutImport_NewImport_InsertsRecord()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);

        await Manager.PutImport(import);

        await AssertRecordExists("import", "file_name", "test");
        await AssertRecordExists("import", "file_type", "L5X");
        await AssertRecordExists("import", "source_type", "CLI");
    }

    [Test]
    public async Task PutImport_ExistingImport_UpdatesStatus()
    {
        var import = Import.Create("test.L5X", SourceType.CLI);
        await Manager.PutImport(import);

        import.Status = ImportStatus.Complete;
        await Manager.PutImport(import);

        await AssertRecordExists("import", "import_status", nameof(ImportStatus.Complete));
    }
}
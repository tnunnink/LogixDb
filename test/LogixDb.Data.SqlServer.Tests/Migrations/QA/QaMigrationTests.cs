namespace LogixDb.Data.SqlServer.Tests.Migrations.QA;

[TestFixture]
public class QaMigrationTests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_To202606120900_CreatesQaSchemaAndTypes()
    {
        await Migrator.Migrate(Connection);

        await AssertSchemaExists("qa");
        await AssertTypeExists("qa", "variables");
        await AssertTypeExists("qa", "validations");
        await AssertTypeExists("qa", "outcome");
    }

    [Test]
    public async Task MigrateUp_To202606121000_CreatesQaValidationRunTable()
    {
        await Migrator.Migrate(Connection);

        await AssertTableExists("qa", "validation_run");
    }

    [Test]
    public async Task MigrateUp_To202606121010_CreatesQaValidationResultTable()
    {
        await Migrator.Migrate(Connection);

        await AssertTableExists("qa", "validation_result");
    }

    [Test]
    public async Task MigrateUp_To202606121100_CreatesQaListValidationsView()
    {
        await Migrator.Migrate(Connection);

        await AssertViewExists("qa", "list_validations");
    }

    [Test]
    public async Task MigrateUp_To202606121110_CreatesQaEmitFailureFunction()
    {
        await Migrator.Migrate(Connection);

        await AssertFunctionExists("qa", "emit_failure");
    }

    [Test]
    public async Task MigrateUp_To202606121120_CreatesQaEmitSuccessFunction()
    {
        await Migrator.Migrate(Connection);

        await AssertFunctionExists("qa", "emit_success");
    }

    [Test]
    public async Task MigrateUp_To202606121130_CreatesQaInspectResultFunction()
    {
        await Migrator.Migrate(Connection);

        await AssertFunctionExists("qa", "inspect_result");
    }

    [Test]
    public async Task MigrateUp_To202606121200_CreatesQaCreateValidationProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "create_validation");
    }

    [Test]
    public async Task MigrateUp_To202606121210_CreatesQaGetVariableProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "get_variable");
    }

    [Test]
    public async Task MigrateUp_To202606121220_CreatesQaGetVariableAsIntProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "get_variable_as_int");
    }

    [Test]
    public async Task MigrateUp_To202606121230_CreatesQaRunValidationProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "run_validation");
    }

    [Test]
    public async Task MigrateUp_To202606121240_CreatesQaRunValidationsProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "run_validations");
    }

    [Test]
    public async Task MigrateUp_To202606161322_CreatesQaGetVariableAsBitProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "get_variable_as_bit");
    }

    [Test]
    public async Task MigrateUp_To202606161323_CreatesQaGetVariableAsRealProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "get_variable_as_real");
    }

    [Test]
    public async Task MigrateUp_To202606161324_CreatesQaGetVariableAsDateProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "get_variable_as_date");
    }

    [Test]
    public async Task MigrateUp_To202606161327_CreatesQaRerunValidationsProcedure()
    {
        await Migrator.Migrate(Connection);

        await AssertProcedureExists("qa", "rerun_validations");
    }
}

namespace LogixDb.Data.SqlServer.Tests.Migrations.QA;

[TestFixture]
public class QaMigrationTests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_To202606120900_CreatesQaSchemaAndTypes()
    {
        await Database.Migrate(202606120900, ComponentOptions.Qa);

        await AssertSchemaExists("qa");
        await AssertTypeExists("qa", "variables");
        await AssertTypeExists("qa", "validations");
        await AssertTypeExists("qa", "outcome");
    }

    [Test]
    public async Task MigrateUp_To202606121000_CreatesQaValidationRunTable()
    {
        await Database.Migrate(202606121000, ComponentOptions.Qa);

        await AssertTableExists("qa", "validation_run");
    }

    [Test]
    public async Task MigrateUp_To202606121010_CreatesQaValidationResultTable()
    {
        await Database.Migrate(202606121010, ComponentOptions.Qa);

        await AssertTableExists("qa", "validation_result");
    }

    [Test]
    public async Task MigrateUp_To202606121100_CreatesQaListValidationsView()
    {
        await Database.Migrate(202606121100, ComponentOptions.Qa);

        await AssertViewExists("qa", "list_validations");
    }

    [Test]
    public async Task MigrateUp_To202606121110_CreatesQaEmitFailureFunction()
    {
        await Database.Migrate(202606121110, ComponentOptions.Qa);

        await AssertFunctionExists("qa", "emit_failure");
    }

    [Test]
    public async Task MigrateUp_To202606121120_CreatesQaEmitSuccessFunction()
    {
        await Database.Migrate(202606121120, ComponentOptions.Qa);

        await AssertFunctionExists("qa", "emit_success");
    }

    [Test]
    public async Task MigrateUp_To202606121130_CreatesQaInspectResultFunction()
    {
        await Database.Migrate(202606121130, ComponentOptions.Qa);

        await AssertFunctionExists("qa", "inspect_result");
    }

    [Test]
    public async Task MigrateUp_To202606121200_CreatesQaCreateValidationProcedure()
    {
        await Database.Migrate(202606121200, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "create_validation");
    }

    [Test]
    public async Task MigrateUp_To202606121210_CreatesQaGetVariableProcedure()
    {
        await Database.Migrate(202606121210, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "get_variable");
    }

    [Test]
    public async Task MigrateUp_To202606121220_CreatesQaGetVariableAsIntProcedure()
    {
        await Database.Migrate(202606121220, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "get_variable_as_int");
    }

    [Test]
    public async Task MigrateUp_To202606121230_CreatesQaRunValidationProcedure()
    {
        await Database.Migrate(202606121230, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "run_validation");
    }

    [Test]
    public async Task MigrateUp_To202606121240_CreatesQaRunValidationsProcedure()
    {
        await Database.Migrate(202606121240, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "run_validations");
    }

    [Test]
    public async Task MigrateUp_To202606161322_CreatesQaGetVariableAsBitProcedure()
    {
        await Database.Migrate(202606161322, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "get_variable_as_bit");
    }

    [Test]
    public async Task MigrateUp_To202606161323_CreatesQaGetVariableAsRealProcedure()
    {
        await Database.Migrate(202606161323, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "get_variable_as_real");
    }

    [Test]
    public async Task MigrateUp_To202606161324_CreatesQaGetVariableAsDateProcedure()
    {
        await Database.Migrate(202606161324, ComponentOptions.Qa);

        await AssertProcedureExists("qa", "get_variable_as_date");
    }
}

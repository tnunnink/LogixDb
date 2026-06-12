using Dapper;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbQaMigrationTests : SqlServerTestFixture
{
    [Test]
    public async Task Migrate_WithQaOption_ShouldCreateAllQaObjects()
    {
        await Database.Migrate(ComponentOptions.QA);

        // Verify Schema
        await AssertSchemaExists("qa");

        // Verify Types
        await AssertTypeExists("qa", "variables");

        // Verify Tables
        await AssertTableExists("qa", "validation_run");
        await AssertTableExists("qa", "validation_result");

        // Verify Views
        await AssertViewExists("qa", "list_validations");

        // Verify Functions
        await AssertFunctionExists("qa", "emit_failure");
        await AssertFunctionExists("qa", "emit_success");
        await AssertFunctionExists("qa", "inspect_result");

        // Verify Procedures
        await AssertProcedureExists("qa", "create_validation");
        await AssertProcedureExists("qa", "get_variable");
        await AssertProcedureExists("qa", "get_variable_as_int");
        await AssertProcedureExists("qa", "run_validation");
        await AssertProcedureExists("qa", "run_validations");
    }

    [Test]
    public async Task Migrate_WithoutQaOption_ShouldNotCreateAnyQaObjects()
    {
        await Database.Migrate(ComponentOptions.None);

        await AssertSchemaDoesNotExist("qa");
    }
}

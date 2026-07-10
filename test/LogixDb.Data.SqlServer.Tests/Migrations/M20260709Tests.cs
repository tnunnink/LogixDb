using Dapper;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260709Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_M20260709_CreatesFunctions()
    {
        await AssertFunctionExists("logix", "tag_path");
        await AssertFunctionExists("logix", "is_atomic");
        await AssertFunctionExists("logix", "default_value");
    }

    [Test]
    public async Task MigrateUp_M20260709_CreatesProcedures()
    {
        await AssertProcedureExists("qa", "execute_validation");
        await AssertProcedureExists("qa", "run_validations");
        await AssertProcedureExists("qa", "create_suite");
        await AssertProcedureExists("qa", "duplicate_suite");
    }

    [Test]
    public async Task CreateSuite_WithValidName_CreatesProcedure()
    {
        await using var connection = await Provider.OpenConnection();
        var suiteName = "Test_Suite_" + Guid.NewGuid().ToString("N");
        
        await connection.ExecuteAsync("EXEC [qa].[create_suite] @suiteName", new { suiteName });

        await AssertProcedureExists("suite", suiteName);
        
        // Cleanup
        await connection.ExecuteAsync($"DROP PROCEDURE [suite].[{suiteName}]");
    }

    [Test]
    public async Task DuplicateSuite_WithValidName_CreatesProcedure()
    {
        await using var connection = await Provider.OpenConnection();
        var suiteName = "Test_Suite_" + Guid.NewGuid().ToString("N");
        var duplicateName = suiteName + "_Dup";
        
        await connection.ExecuteAsync("EXEC [qa].[create_suite] @suiteName", new { suiteName });
        await connection.ExecuteAsync("EXEC [qa].[duplicate_suite] @suiteName, @duplicateName", new { suiteName, duplicateName });

        await AssertProcedureExists("suite", suiteName);
        await AssertProcedureExists("suite", duplicateName);

        // Verify content (simple check if it exists and has the same definition modulo name)
        var originalDefinition = await connection.ExecuteScalarAsync<string>("SELECT OBJECT_DEFINITION(OBJECT_ID(@name))", new { name = $"[suite].[{suiteName}]" });
        var duplicateDefinition = await connection.ExecuteScalarAsync<string>("SELECT OBJECT_DEFINITION(OBJECT_ID(@name))", new { name = $"[suite].[{duplicateName}]" });
        
        Assert.That(duplicateDefinition, Is.Not.Null);
        Assert.That(duplicateDefinition, Is.EqualTo(originalDefinition.Replace(suiteName, duplicateName)));

        // Cleanup
        await connection.ExecuteAsync($"DROP PROCEDURE [suite].[{suiteName}]");
        await connection.ExecuteAsync($"DROP PROCEDURE [suite].[{duplicateName}]");
    }

    [Test]
    public async Task LatestVersionId_WithTargetKey_ReturnsExpectedId()
    {
        await using var connection = await Provider.OpenConnection();
        var targetKey = "Test_Target_" + Guid.NewGuid().ToString("N");
        
        // Insert test data
        await connection.ExecuteAsync(@"
            INSERT INTO logix.target (target_key, target_name) VALUES (@targetKey, @targetKey);
            DECLARE @targetId INT = SCOPE_IDENTITY();
            INSERT INTO logix.target_version (target_id, version_number) VALUES (@targetId, 1);
            INSERT INTO logix.target_version (target_id, version_number) VALUES (@targetId, 2);
        ", new { targetKey });

        var expectedId = await connection.ExecuteScalarAsync<int>(@"
            SELECT TOP 1 version_id 
            FROM logix.target_version tv 
            JOIN logix.target t ON t.target_id = tv.target_id 
            WHERE t.target_key = @targetKey 
            ORDER BY version_number DESC", new { targetKey });

        var result = await connection.ExecuteScalarAsync<int>("SELECT logix.latest_version_id(@targetKey)", new { targetKey });
        
        Assert.That(result, Is.EqualTo(expectedId));
    }

    [Test]
    public async Task ExecuteValidation_WithNoResults_AssumesSuccess()
    {
        await using var connection = await Provider.OpenConnection();
        var validationName = "Test_Validation_" + Guid.NewGuid().ToString("N");
        
        // Create a validation that does nothing
        await connection.ExecuteAsync($@"
            CREATE PROCEDURE [qa].[{validationName}]
                @vars qa.variables READONLY
            AS
            BEGIN
                SET NOCOUNT ON;
            END");

        try
        {
            await connection.ExecuteAsync(@"
                DECLARE @vars qa.variables;
                EXEC [qa].[execute_validation] @vars, @validationName", 
                new { validationName = $"[qa].[{validationName}]" });

            var result = await connection.QuerySingleAsync(@"
                SELECT TOP 1 is_success, result_message 
                FROM [qa].validation_result 
                WHERE validation_name = @name 
                ORDER BY result_id DESC", 
                new { name = $"[qa].[{validationName}]" });

            Assert.That(result.is_success, Is.True);
            Assert.That(result.result_message, Is.EqualTo("Validation completed successfully with no results"));
        }
        finally
        {
            await connection.ExecuteAsync($"DROP PROCEDURE [qa].[{validationName}]");
        }
    }

    [Test]
    public async Task RunValidations_WithFailingValidation_SetsStatusToFailed()
    {
        await using var connection = await Provider.OpenConnection();
        var validationName = "Test_Fail_" + Guid.NewGuid().ToString("N");
        var runName = "Test_Run_" + Guid.NewGuid().ToString("N");
        
        // Create a failing validation
        await connection.ExecuteAsync($@"
            CREATE PROCEDURE [qa].[{validationName}]
                @vars qa.variables READONLY
            AS
            BEGIN
                SELECT CAST(0 AS BIT) AS is_success, 'Failed' AS result_message, '[]' AS result_details;
            END");

        try
        {
            await connection.ExecuteAsync(@"
                DECLARE @vars qa.variables;
                DECLARE @vals qa.validations;
                INSERT INTO @vals VALUES (@validationName);
                EXEC [qa].[run_validations] @vars, @vals, @runName", 
                new { validationName = $"[qa].[{validationName}]", runName });

            var runStatus = await connection.ExecuteScalarAsync<string>(@"
                SELECT TOP 1 run_status 
                FROM [qa].validation_run 
                WHERE run_name = @runName 
                ORDER BY run_id DESC", 
                new { runName });

            Assert.That(runStatus, Is.EqualTo("Failed"));
        }
        finally
        {
            await connection.ExecuteAsync($"DROP PROCEDURE [qa].[{validationName}]");
        }
    }

    [TestCase("Tag", "")]
    [TestCase("Tag.Member", "Member")]
    [TestCase("Tag[0]", "[0]")]
    [TestCase("Tag.Member[0]", "Member[0]")]
    [TestCase("Tag[0].Member", "[0].Member")]
    public async Task TagPath_WithInput_ReturnsExpectedPath(string tagName, string expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.tag_path(@tagName)", new { tagName });
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("BOOL", true)]
    [TestCase("SINT", true)]
    [TestCase("INT", true)]
    [TestCase("DINT", true)]
    [TestCase("LINT", true)]
    [TestCase("REAL", true)]
    [TestCase("LREAL", true)]
    [TestCase("USINT", true)]
    [TestCase("UINT", true)]
    [TestCase("UDINT", true)]
    [TestCase("ULINT", true)]
    [TestCase("DT", true)]
    [TestCase("LDT", true)]
    [TestCase("TIME32", true)]
    [TestCase("TIME", true)]
    [TestCase("LTIME", true)]
    [TestCase("STRING", false)]
    [TestCase("MY_STRUCT", false)]
    [TestCase("MyCustomType", false)]
    public async Task IsAtomic_WithInput_ReturnsExpected(string dataType, bool expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<bool>("SELECT logix.is_atomic(@dataType)", new { dataType });
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("BOOL", "0")]
    [TestCase("SINT", "0")]
    [TestCase("INT", "0")]
    [TestCase("DINT", "0")]
    [TestCase("LINT", "0")]
    [TestCase("USINT", "0")]
    [TestCase("UINT", "0")]
    [TestCase("UDINT", "0")]
    [TestCase("ULINT", "0")]
    [TestCase("DT", "0")]
    [TestCase("LDT", "0")]
    [TestCase("TIME", "0")]
    [TestCase("TIME32", "0")]
    [TestCase("LTIME", "0")]
    [TestCase("REAL", "0.0")]
    [TestCase("LREAL", "0.0")]
    [TestCase("STRING", null)]
    public async Task DefaultValue_WithInput_ReturnsExpectedValue(string dataType, string? expected)
    {
        await using var connection = await Provider.OpenConnection();
        var result = await connection.ExecuteScalarAsync<string>("SELECT logix.default_value(@dataType)", new { dataType });
        Assert.That(result, Is.EqualTo(expected));
    }
}

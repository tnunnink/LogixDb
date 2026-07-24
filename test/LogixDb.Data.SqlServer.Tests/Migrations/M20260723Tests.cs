using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260723Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_M20260723_CreatesObjects()
    {
        await AssertFunctionExists("qa", "hash");
        await AssertProcedureExists("qa", "generate_approval");
        await AssertProcedureExists("qa", "execute_validation");
        await AssertProcedureExists("qa", "get_override_variable");
    }

    [Test]
    public async Task Hash_WithInput_ReturnsExpectedHash()
    {
        await using var connection = await Provider.OpenConnection();
        var input = "test data";
        
        var result = await connection.ExecuteScalarAsync<string>("SELECT qa.hash(@input)", new { input });
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(64));
        // SHA256 of N'test data' (UTF-16LE)
        Assert.That(result, Is.EqualTo("1F4A14C8F69FFA84088C6FF83F68E09271D2A63C92ECD0DC0781E84E7BA4DCD7"));
    }

    [Test]
    public async Task GenerateApproval_WithFailedValidation_ReturnsApprovalString()
    {
        await using var connection = await Provider.OpenConnection();
        var runId = await CreateTestRun(connection);
        
        // Setup: Create a failed validation result
        var validationName = "Test.Validation";
        var resultDetails = "{\"error\": \"failed\"}";
        var expectedHash = await connection.ExecuteScalarAsync<string>("SELECT qa.hash(@resultDetails)", new { resultDetails });
        
        var resultId = await connection.ExecuteScalarAsync<long>(
            "INSERT INTO qa.validation_result (run_id, validation_name, is_success, result_message, result_details) OUTPUT INSERTED.result_id VALUES (@runId, @validationName, 0, 'Failed', @resultDetails)",
            new { runId, validationName, resultDetails });

        var approval = await connection.ExecuteScalarAsync<string>("EXEC [qa].[generate_approval] @resultId", new { resultId });

        var expectedApproval = $"('{validationName}', '{expectedHash}')";
        Assert.That(approval, Is.EqualTo(expectedApproval));
    }

    [Test]
    public async Task GenerateApproval_WithSuccessValidation_Throws()
    {
        await using var connection = await Provider.OpenConnection();
        var runId = await CreateTestRun(connection);
        
        var resultId = await connection.ExecuteScalarAsync<long>(
            "INSERT INTO qa.validation_result (run_id, validation_name, is_success, result_message) OUTPUT INSERTED.result_id VALUES (@runId, 'Test.Success', 1, 'Passed')",
            new { runId });

        var ex = Assert.ThrowsAsync<SqlException>(async () =>
            await connection.ExecuteAsync("EXEC [qa].[generate_approval] @resultId", new { resultId }));
        
        Assert.That(ex.Message, Contains.Substring("Only failed validations can be used to generate an approval"));
    }

    [Test]
    public async Task ExecuteValidation_WithMissingProcedure_InsertsFailureAndThrows()
    {
        await using var connection = await Provider.OpenConnection();
        var runId = await CreateTestRun(connection);
        var validationName = "non_existent_procedure";
        
        var vars = new DataTable();
        vars.Columns.Add("variable_name", typeof(string));
        vars.Columns.Add("variable_value", typeof(string));

        var ex = Assert.ThrowsAsync<SqlException>(async () =>
            await connection.ExecuteAsync("[qa].[execute_validation]", 
                new { vars = vars.AsTableValuedParameter("qa.variables"), validation_name = validationName, run_id = runId }, 
                commandType: CommandType.StoredProcedure));
        
        Assert.That(ex.Message, Contains.Substring("Validation procedure does not exist"));
        
        var result = await connection.QuerySingleAsync(
            "SELECT is_success, result_message FROM qa.validation_result WHERE validation_name = @validationName AND run_id = @runId",
            new { validationName, runId });
            
        Assert.That(result.is_success, Is.False);
        Assert.That(result.result_message, Is.EqualTo("Validation procedure does not exist"));
    }

    private async Task<long> CreateTestRun(IDbConnection connection)
    {
        return await connection.ExecuteScalarAsync<long>(
            "INSERT INTO qa.validation_run (run_name, run_status, variables_data, variables_hash) OUTPUT INSERTED.run_id VALUES (@name, @status, @data, @hash)",
            new { name = "Test Run", status = "Running", data = "{}", hash = new byte[32] });
    }

    [Test]
    public async Task GetOverrideVariable_WithMatchingObject_ReturnsValue()
    {
        await using var connection = await Provider.OpenConnection();
        
        // Using a known object, e.g., qa.hash
        var objectId = await connection.ExecuteScalarAsync<int>("SELECT OBJECT_ID('qa.hash')");
        var expectedValue = "OverrideValue";
        
        var vars = new DataTable();
        vars.Columns.Add("variable_name", typeof(string));
        vars.Columns.Add("variable_value", typeof(string));
        vars.Rows.Add("qa.hash", expectedValue);

        var parameters = new DynamicParameters();
        parameters.Add("@vars", vars.AsTableValuedParameter("qa.variables"));
        parameters.Add("@id", objectId);
        parameters.Add("@variable", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

        await connection.ExecuteAsync("[qa].[get_override_variable]", parameters, commandType: CommandType.StoredProcedure);

        var result = parameters.Get<string>("@variable");
        Assert.That(result, Is.EqualTo(expectedValue));
    }
}

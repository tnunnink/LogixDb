using System.Data;
using Dapper;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.SqlServer.Tests;

/// <summary>
/// Represents a base test fixture for managing a SQL Server container and
/// a corresponding database instance for integration tests. This class
/// sets up and tears down the necessary resources for running tests
/// against a SQL Server database, using Testcontainers.MsSql for
/// container management.
/// </summary>
public abstract class SqlServerTestFixture
{
    /// <summary>
    /// Provides an instance of the database migrator to perform schema and data migration
    /// operations against a SQL Server database during integration tests.
    /// </summary>
    /// <remarks>
    /// The migrator is used to apply migrations and ensure that the database schema is
    /// correctly configured and up to date. It relies on the implementation of the
    /// <c>IDbMigrator</c> interface, which defines the contract for executing migration processes.
    /// </remarks>
    protected static IDbMigrator Migrator => new SqlServerMigrator();

    /// <summary>
    /// Provides the database connection details used to interact with the
    /// SQL Server test container during integration tests.
    /// </summary>
    /// <remarks>
    /// This property returns an instance of <c>DbConnectionInfo</c>, describing the
    /// configuration necessary for establishing a connection to the SQL Server test database.
    /// The information includes provider, server address, credentials, encryption settings, and other
    /// connection-specific attributes.
    /// </remarks>
    protected static DbConnectionInfo Connection => SqlServerTestContainer.Connection;

    /// <summary>
    /// Provides an instance of the database manager to interact with the SQL Server test container.
    /// This property is used for database operations, including migrations, queries, and imports
    /// during integration and performance tests.
    /// </summary>
    /// <remarks>
    /// The database manager instance is initialized using the provider from the SQL Server test container.
    /// It is designed to facilitate testing of database schemas, data integrity, and performance in a controlled
    /// environment.
    /// </remarks>
    protected static IDbManager Manager => new DbManager(SqlServerTestContainer.Provider);

    /// <summary>
    /// Asserts that the specified table contains exactly the expected number of records.
    /// </summary>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="expectedCount">The expected number of records in the table.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    protected static async Task AssertRecordCount(string tableName, int expectedCount)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleAsync<int>($"SELECT COUNT(*) FROM {tableName}");

        Assert.That(result, Is.EqualTo(expectedCount),
            $"Expected {expectedCount} records in table '{tableName}', but found {result}");
    }

    /*
    /// <summary>
    /// Cleans up after each test by dropping the database instance used during the test.
    /// This ensures the database is reset and no residual data persists between tests,
    /// maintaining isolation and consistency across test cases.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [TearDown]
    protected virtual async Task TearDown()
    {
        await Database.Drop();
    }*/

    /// <summary>
    /// Retrieves the current size of the database in megabytes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The result contains the size of the database in megabytes as an integer.</returns>
    protected static async Task<decimal> GetDatabaseSize()
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        return await connection.QuerySingleAsync<decimal>(
            """
            SELECT CAST(SUM(FILEPROPERTY(name, 'SpaceUsed')) * 8 / 1024.0 AS DECIMAL(10,2)) AS SpaceUsedMB
            FROM sys.database_files
            WHERE type_desc = 'ROWS';
            """);
        // """
        // SELECT CAST(SUM(size) * 8 / 1024 AS DECIMAL(10,2)) AS SizeMB
        // FROM sys.master_files
        // WHERE database_id = DB_ID()
        // GROUP BY database_id;
        // """);
    }

    /// <summary>
    /// Asserts that a record exists in a specified table with a specific value for a given column.
    /// An exception is thrown if no matching record is found.
    /// </summary>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="columnName">The name of the column to inspect for the expected value.</param>
    /// <param name="expected">The value to match in the specified column.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    protected static async Task AssertRecordExists(string tableName, string columnName, object expected)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleAsync<int>(
            $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @expected",
            new { expected }
        );

        Assert.That(result, Is.GreaterThanOrEqualTo(1),
            $"No record with '{columnName}={expected}' exists in table '{tableName}'"
        );
    }


    /// <summary>
    /// Verifies that the specified table does not exist in the database by querying the
    /// INFORMATION_SCHEMA.TABLES view. If the table is found, an exception is thrown.
    /// </summary>
    /// <param name="tableName">The name of the table to verify non-existence for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="AssertionException">Thrown if the specified table is found in the database.</exception>
    protected static async Task AssertTableDoesNotExists(string tableName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleOrDefaultAsync<int>(
            """
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = @tableName
            """,
            new { tableName }
        );

        if (result == 1)
            throw new AssertionException($"Table '{tableName}' was found in the database.");
    }

    /// <summary>
    /// Asserts that a record does not exist in a specified table with a specific value for a given column.
    /// An exception is thrown if a matching record is found.
    /// </summary>
    protected static async Task AssertRecordDoesNotExist(string tableName, string columnName, object expected)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleAsync<int>(
            $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @expected",
            new { expected }
        );

        Assert.That(result, Is.EqualTo(0),
            $"Record with '{columnName}={expected}' exists in table '{tableName}' but should not"
        );
    }

    /// <summary>
    /// Verifies the existence of a specific function in the database by querying the
    /// INFORMATION_SCHEMA.ROUTINES view. If the function does not exist, an exception is thrown.
    /// </summary>
    /// <param name="schemaName">The schema where the function is located.</param>
    /// <param name="functionName">The name of the function to check for existence in the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="AssertionException">Thrown if the specified function does not exist in the database.</exception>
    protected static async Task AssertFunctionExists(string schemaName, string functionName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleOrDefaultAsync<int>(
            """
            SELECT 1
            FROM INFORMATION_SCHEMA.ROUTINES
            WHERE ROUTINE_SCHEMA = @schemaName AND ROUTINE_NAME = @functionName AND ROUTINE_TYPE = 'FUNCTION'
            """,
            new { schemaName, functionName }
        );

        if (result < 1)
            throw new AssertionException($"Function '{schemaName}.{functionName}' was not found in the database.");
    }

    /// <summary>
    /// Verifies the existence of a specific function in the database by querying the
    /// INFORMATION_SCHEMA.ROUTINES view. If the function does not exist, an exception is thrown.
    /// </summary>
    /// <param name="functionName">The name of the function to check for existence in the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="AssertionException">Thrown if the specified function does not exist in the database.</exception>
    protected static async Task AssertFunctionExists(string functionName)
    {
        await AssertFunctionExists("dbo", functionName);
    }

    /// <summary>
    /// Verifies the existence of a specific table in the database by querying the
    /// INFORMATION_SCHEMA.TABLES view. If the table does not exist, an exception is thrown.
    /// </summary>
    /// <param name="schemaName">The schema where the table is located.</param>
    /// <param name="tableName">The name of the table to check for existence in the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="AssertionException">Thrown if the specified table does not exist in the database.</exception>
    protected static async Task AssertTableExists(string schemaName, string tableName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleOrDefaultAsync<int>(
            """
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @schemaName AND TABLE_NAME = @tableName
            """,
            new { schemaName, tableName }
        );

        if (result < 1)
            throw new AssertionException($"Table '{schemaName}.{tableName}' was not found in the database.");
    }

    /// <summary>
    /// Verifies the existence of a specific table in the database by querying the
    /// INFORMATION_SCHEMA.TABLES view. If the table does not exist, an exception is thrown.
    /// </summary>
    /// <param name="tableName">The name of the table to check for existence in the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="AssertionException">Thrown if the specified table does not exist in the database.</exception>
    protected static async Task AssertTableExists(string tableName)
    {
        await AssertTableExists("dbo", tableName);
    }

    /// <summary>
    /// Verifies the existence of a specific schema in the database.
    /// </summary>
    /// <param name="schemaName">The name of the schema to check for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task AssertSchemaExists(string schemaName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            "SELECT 1 FROM sys.schemas WHERE name = @schemaName",
            new { schemaName }
        );

        if (result < 1)
            throw new AssertionException($"Schema '{schemaName}' was not found in the database.");
    }

    /// <summary>
    /// Verifies the existence of a specific table-valued type in the database.
    /// </summary>
    /// <param name="schemaName">The schema where the type is located.</param>
    /// <param name="typeName">The name of the type to check for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task AssertTypeExists(string schemaName, string typeName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            """
            SELECT 1 
            FROM sys.table_types t
            JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @schemaName AND t.name = @typeName
            """,
            new { schemaName, typeName }
        );

        if (result < 1)
            throw new AssertionException($"Table type '{schemaName}.{typeName}' was not found in the database.");
    }

    /// <summary>
    /// Verifies the existence of a specific view in the database.
    /// </summary>
    /// <param name="schemaName">The schema where the view is located.</param>
    /// <param name="viewName">The name of the view to check for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task AssertViewExists(string schemaName, string viewName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            """
            SELECT 1
            FROM INFORMATION_SCHEMA.VIEWS
            WHERE TABLE_SCHEMA = @schemaName AND TABLE_NAME = @viewName
            """,
            new { schemaName, viewName }
        );

        if (result < 1)
            throw new AssertionException($"View '{schemaName}.{viewName}' was not found in the database.");
    }

    /// <summary>
    /// Verifies the existence of a specific stored procedure in the database.
    /// </summary>
    /// <param name="schemaName">The schema where the procedure is located.</param>
    /// <param name="procedureName">The name of the procedure to check for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task AssertProcedureExists(string schemaName, string procedureName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            """
            SELECT 1
            FROM INFORMATION_SCHEMA.ROUTINES
            WHERE ROUTINE_SCHEMA = @schemaName AND ROUTINE_NAME = @procedureName AND ROUTINE_TYPE = 'PROCEDURE'
            """,
            new { schemaName, procedureName }
        );

        if (result < 1)
            throw new AssertionException($"Procedure '{schemaName}.{procedureName}' was not found in the database.");
    }

    /// <summary>
    /// Verifies the existence and data type for the specified column in a given table within the database.
    /// Throws an assertion exception if the column does not exist or its data type does not match the expected type.
    /// </summary>
    /// <param name="tableName">The name of the table in which the column is located.</param>
    /// <param name="columnName">The name of the column to verify.</param>
    /// <param name="columnType">The expected data type of the column.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task AssertColumnDefinition(string tableName, string columnName, string columnType)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleOrDefaultAsync<string>(
            """
            SELECT DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName
            """,
            new { tableName, columnName }
        );

        if (result is null)
            throw new AssertionException($"Column '{columnName}' was not found in table '{tableName}'.");

        if (!string.Equals(result, columnType, StringComparison.OrdinalIgnoreCase))
            throw new AssertionException(
                $"Expected column '{columnName}' in table '{tableName}' to have type '{columnType}', but found type '{result}'.");
    }

    /// <summary>
    /// Verifies that the specified column in the given table is defined as a primary key.
    /// Throws an AssertionException if the primary key constraint is not found.
    /// </summary>
    /// <param name="tableName">The name of the table to check for the primary key.</param>
    /// <param name="columnName">The name of the column expected to be the primary key.</param>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    protected static async Task AssertPrimaryKey(string tableName, string columnName)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QuerySingleAsync<int>(
            """
            SELECT 1 
            FROM 
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS t, 
                INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE c 
            WHERE 
                c.Constraint_Name = t.Constraint_Name
                AND c.Table_Name = t.Table_Name
                AND t.Constraint_Type = 'PRIMARY KEY'
                AND c.Table_Name = @tableName
                AND c.COLUMN_NAME = @columnName
            """,
            new { tableName, columnName }
        );

        if (result < 1)
            throw new AssertionException(
                $"Expected primary key on column '{columnName}' in table '{tableName}', but none was found.");
    }

    /// <summary>
    /// Verifies that a foreign key constraint exists between the specified columns and tables in the database.
    /// </summary>
    /// <param name="fromTable">The name of the table containing the foreign key column.</param>
    /// <param name="fromColumn">The name of the column in the source table that forms the foreign key.</param>
    /// <param name="toTable">The name of the table containing the referenced primary key column.</param>
    /// <param name="toColumn">The name of the column in the target table that is referenced by the foreign key.</param>
    /// <returns>A task representing the asynchronous operation, which throws an <see cref="AssertionException"/> if the foreign key does not exist.</returns>
    protected static async Task AssertForeignKey(string fromTable, string fromColumn, string toTable, string toColumn)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var result = await connection.QueryFirstOrDefaultAsync<int>(
            """
            SELECT 1
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE fk 
                ON rc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE pk
                ON rc.UNIQUE_CONSTRAINT_NAME = pk.CONSTRAINT_NAME
                AND pk.ORDINAL_POSITION = fk.ORDINAL_POSITION
            WHERE fk.TABLE_NAME = @fromTable 
              AND fk.COLUMN_NAME = @fromColumn 
              AND pk.TABLE_NAME = @toTable
              AND pk.COLUMN_NAME = @toColumn;
            """,
            new { fromTable, fromColumn, toTable, toColumn }
        );

        if (result < 1)
            throw new AssertionException(
                $"Expected FK {fromTable}({fromColumn}) -> {toTable}({toColumn}), but none was found.");
    }

    /// <summary>
    /// Asserts that an index exists on the specified table with the given columns.
    /// Throws an <c>AssertionException</c> if no matching index is found.
    /// </summary>
    /// <param name="tableName">The name of the table on which to check for the existence of the index.</param>
    /// <param name="columns">A collection of column names that should define the index.</param>
    /// <returns>A task that represents the completion of the assertion operation.</returns>
    /// <exception cref="AssertionException">Thrown if no index matches the specified columns on the table.</exception>
    protected static async Task AssertIndex(string tableName, params string[] columns)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var indexes = (await connection.QueryAsync<string>(
            """
            SELECT i.name
            FROM sys.indexes i
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = @tableName
              AND i.is_primary_key = 0
              AND i.type > 0
            """,
            new { tableName }
        )).ToArray();

        var hasMatch = await HasMatchingIndex(connection, tableName, columns, indexes);

        if (!hasMatch)
            throw new AssertionException(
                $"""
                 Expected index on '{tableName}' for ({string.Join(", ", columns)}),
                 but none was found. Found indexes: {string.Join(", ", indexes)}
                 """
            );
    }

    /// <summary>
    /// Asserts that a UNIQUE index exists on the specified table with the given columns.
    /// Throws an <c>AssertionException</c> if no matching UNIQUE index is found.
    /// </summary>
    /// <param name="tableName">The name of the table on which to check for the existence of the UNIQUE index.</param>
    /// <param name="columns">A collection of column names that should define the UNIQUE index.</param>
    /// <returns>A task that represents the completion of the assertion operation.</returns>
    /// <exception cref="AssertionException">Thrown if no UNIQUE index matches the specified columns on the table.</exception>
    protected static async Task AssertUniqueIndex(string tableName, params string[] columns)
    {
        await using var connection = await SqlServerTestContainer.Provider.OpenConnection();

        var indexes = (await connection.QueryAsync<string>(
            """
            SELECT i.name
            FROM sys.indexes i
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = @tableName
              AND i.is_unique = 1
              AND i.is_primary_key = 0
              AND i.type > 0
            """,
            new { tableName }
        )).ToArray();

        var hasMatch = await HasMatchingIndex(connection, tableName, columns, indexes);

        if (!hasMatch)
            throw new AssertionException(
                $"""
                 Expected UNIQUE index on '{tableName}' for ({string.Join(", ", columns)}),
                 but none was found. Found unique indexes: {string.Join(", ", indexes)}
                 """
            );
    }

    /// <summary>
    /// Checks if there is an existing index in the database that matches the specified column order.
    /// </summary>
    /// <param name="connection">The database connection used to query index information.</param>
    /// <param name="tableName">The name of the table containing the indexes.</param>
    /// <param name="columns">The array of column names that defines the desired order and structure of the index.</param>
    /// <param name="indexes">The array of existing index names associated with the table.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is <c>true</c>
    /// if an index matching the specified column order is found; otherwise, <c>false</c>.
    /// </returns>
    private static async Task<bool> HasMatchingIndex(IDbConnection connection,
        string tableName,
        string[] columns,
        string[] indexes
    )
    {
        foreach (var indexName in indexes)
        {
            var columnNames = await connection.QueryAsync<string>(
                """
                SELECT c.name
                FROM sys.index_columns ic
                INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                INNER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                INNER JOIN sys.tables t ON i.object_id = t.object_id
                WHERE t.name = @tableName
                  AND i.name = @indexName
                ORDER BY ic.key_ordinal
                """,
                new { tableName, indexName }
            );

            if (columns.SequenceEqual(columnNames)) return true;
        }

        return false;
    }
}
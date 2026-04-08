using FluentAssertions;

namespace LogixDb.Data.Tests;

[TestFixture]
public class DbConnectionInfoTests
{
    [Test]
    public void Parse_SqliteConnection_ShouldReturnSqliteProvider()
    {
        const string connectionString = "test.db";
        var connection = DbConnectionInfo.Parse(connectionString);

        connection.Provider.Should().Be(DbProvider.Sqlite);
        connection.Source.Should().Be(connectionString);
    }

    [Test]
    public void Parse_SqliteConnectionPath_ShouldReturnSqliteProvider()
    {
        const string connectionString = @"C:\Data\test.db";
        var connection = DbConnectionInfo.Parse(connectionString);

        connection.Provider.Should().Be(DbProvider.Sqlite);
        connection.Source.Should().Be(connectionString);
    }

    [Test]
    public void Parse_SqlServerConnection_ShouldReturnSqlServerProviderWithParts()
    {
        const string connectionString =
            "MyDatabase@MyServer;User=admin;Password=secret;Port=1234;Trust=true;Encrypt=true";
        var connection = DbConnectionInfo.Parse(connectionString);

        connection.Provider.Should().Be(DbProvider.SqlServer);
        connection.Database.Should().Be("MyDatabase");
        connection.Source.Should().Be("MyServer");
        connection.User.Should().Be("admin");
        connection.Password.Should().Be("secret");
        connection.Port.Should().Be(1234);
        connection.Trust.Should().BeTrue();
        connection.Encrypt.Should().BeTrue();
    }

    [Test]
    public void Parse_SqlServerMinimalConnection_ShouldReturnSqlServerProviderWithDefaults()
    {
        const string connectionString = "MyDatabase@MyServer";
        var connection = DbConnectionInfo.Parse(connectionString);

        connection.Provider.Should().Be(DbProvider.SqlServer);
        connection.Database.Should().Be("MyDatabase");
        connection.Source.Should().Be("MyServer");
        connection.Port.Should().Be(1433); // Default
        connection.Trust.Should().BeFalse();
        connection.Encrypt.Should().BeFalse();
    }

    [Test]
    public void Parse_InvalidFormat_ShouldThrowFormatException()
    {
        const string connectionString = "MyDatabase@MyServer;InvalidPart";

        Action act = () => DbConnectionInfo.Parse(connectionString);

        act.Should().Throw<FormatException>();
    }

    [Test]
    public void Parse_UnableToInfer_ShouldThrowArgumentException()
    {
        const string connectionString = "InvalidConnection";

        Action act = () => DbConnectionInfo.Parse(connectionString);

        act.Should().Throw<ArgumentException>();
    }
}
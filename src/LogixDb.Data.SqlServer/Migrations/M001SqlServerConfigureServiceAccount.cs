using FluentMigrator;
using JetBrains.Annotations;

// ReSharper disable StringLiteralTypo

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202602262030, @"Grants permissions to the NT SERVICE\LogixDb virtual account used by the windows service.")]
public class M001SqlServerConfigureServiceAccount : Migration
{
    public override void Up()
    {
        // 1. Create Login for the Virtual Account (Server level)
        Execute.Sql(
            """
            IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NT SERVICE\LogixDb')
            BEGIN
                DECLARE @sql NVARCHAR(MAX);
                SET @sql = N'CREATE LOGIN [NT SERVICE\LogixDb] FROM WINDOWS WITH DEFAULT_DATABASE = ' + QUOTENAME(DB_NAME());
                EXEC sp_executesql @sql;
            END
            """
        );

        // 2. Create User and Grant Roles (Database level)
        Execute.Sql(
            """
            IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'LogixServiceUser')
            BEGIN
                CREATE USER [LogixServiceUser] FOR LOGIN [NT SERVICE\LogixDb];
                ALTER ROLE [db_datareader] ADD MEMBER [LogixServiceUser];
                ALTER ROLE [db_datawriter] ADD MEMBER [LogixServiceUser];
                ALTER ROLE [db_owner] ADD MEMBER [LogixServiceUser];
            END
            """
        );
    }

    public override void Down()
    {
        Execute.Sql("DROP USER [LogixServiceUser];");
        Execute.Sql("DROP LOGIN [NT SERVICE\\LogixDb];");
    }
}
using FluentMigrator;
using JetBrains.Annotations;
using L5Sharp.Core;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260524;

[UsedImplicitly]
//[Migration(202605241400, "Seeds native data types using predefined structures from L5Sharp.")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic, MigrationTag.DataType)]
public class M202605241400SeedNativeDataTypes : AutoReversingMigration
{
    public override void Up()
    {
    }
}
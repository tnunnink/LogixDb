using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;
using DbUp.Engine;
using LogixDb.Data.Maps;
using LogixDb.Data.Resources;
using SqlKata;
using SqlKata.Compilers;

namespace LogixDb.Data.Sqlite.Scripts.Migrations;

public class Logix_202606180900_SeedOperands : IScript
{
    private static readonly OperandMap Map = new();

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        var records = SeedResource.LoadOperands();
        var compiler = new SqliteCompiler();

        var queries = records.Select(r =>
        {
            var hash = Map.ComputeHash(r);
            var query = new Query("operand").AsInsert(new
            {
                instruction_key = r.Key,
                operand_index = r.Index,
                operand_name = r.Name,
                operand_type = r.Type,
                operand_description = r.Description,
                is_destructive = r.Destructive ? 1 : 0,
                is_native = r.Native ? 1 : 0,
                record_hash = hash
            });
            
            return compiler.Compile(query).ToString(); 
        });

        return string.Join(";\n", queries);
    }
}
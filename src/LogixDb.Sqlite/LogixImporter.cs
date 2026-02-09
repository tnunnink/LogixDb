using System.Data;
using Dapper;
using L5Sharp.Core;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Cli.Common;

/// <summary>
/// 
/// </summary>
/// <param name="targetKey"></param>
/// <param name="source"></param>
/// <param name="connection"></param>
internal class LogixImporter(string targetKey, L5X source, IDbConnection connection)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task Import(CancellationToken token)
    {
        //wrap all in same transaction for performance.
        using var transaction = connection.BeginTransaction();

        // First step is to create the snapshot (and target if does not exist).
        var snapshotId = await CreateSnapshot(transaction, token);

        //Run all import steps in order (order not super important since not a ton of FKs)
        await ImportController(snapshotId, transaction, token);
        await ImportTags(snapshotId, transaction, token);

        //int is just a placeholder not sure what make sense to return yet.
    }

    private async Task<int> CreateSnapshot(IDbTransaction transaction, CancellationToken token)
    {
        const string insertTarget =
            """
            INSERT INTO target (target_key, target_type, target_name, is_partial)  
            VALUES (@TargetKey, @TargetType, @TargetName, @IsPartial)
            """;
        
        const string insertSnapshot =
            """
            INSERT INTO snapshot (target_id, source_revision, source_hash, exported_on, imported_on, imported_by, imported_from, options) 
            VALUES (@TargetId, @SourceRevision)
            """;

        var snapshot = new { };

        return await connection.ExecuteScalarAsync<int>(insertSnapshot, snapshot, transaction);
    }


    private Task ImportController(int snapshotId, IDbTransaction transaction, CancellationToken token)
    {
        //todo use connection, source, snapshotid, transaction, and token to map controller data into database
        throw new NotImplementedException();
    }

    private Task ImportTags(int snapshotId, IDbTransaction transaction, CancellationToken token)
    {
        //todo use connection, source, snapshotid, transaction, and token to map tag data into database
        throw new NotImplementedException();
    }
}
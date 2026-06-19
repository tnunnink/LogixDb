using Dapper;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data;

/// <summary>
/// Manages SQLite database operations such as migrations, connections, and database removal.
/// </summary>
/// <remarks>
/// This class is a concrete implementation of the <c>ILogixDbManager</c> interface, designed to
/// work with SQLite databases. It facilitates operations like running database migrations,
/// connecting to the database, and managing the database file lifecycle.
/// </remarks>
public sealed class DbManager(IDbProvider provider) : IDbManager
{
    /// <inheritdoc />
    public async Task<IEnumerable<Target>> ListTargets(string? targetKey = null, CancellationToken token = default)
    {
        await using var connection = await provider.OpenConnection(token);
        var script = provider.GetScript(ScriptName.ListTargets);
        return await connection.QueryAsync<Target>(script, new { TargetKey = targetKey });
    }

    /// <inheritdoc />
    public async Task<Target?> GetTarget(string targetKey, int version = 0, CancellationToken token = default)
    {
        await using var connection = await provider.OpenConnection(token);

        // If the version is zero, then just get the latest for the specific target
        var scriptName = version > 0 ? ScriptName.GetTargetByVersion : ScriptName.GetTargetByLatest;
        var script = provider.GetScript(scriptName);

        return await connection.QuerySingleOrDefaultAsync<Target>(script,
            new { TargetKey = targetKey, VersionNumber = version }
        );
    }

    /// <inheritdoc />
    public async Task ImportTarget(Target target, CancellationToken token = default)
    {
        var writer = provider.GetWriter();
        await writer.WriteAsync(target, token);
    }

    /// <inheritdoc />
    public Task DeleteTarget(string targetKey, CancellationToken token = default)
    {
        var script = provider.GetScript(ScriptName.DeleteTarget);

        return ExecuteSqliteScriptAsync(
            script,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersion(string targetKey, int versionNumber, CancellationToken token = default)
    {
        var script = provider.GetScript(ScriptName.DeleteVersion);

        return ExecuteSqliteScriptAsync(
            script,
            new { TargetKey = targetKey, VersionNumber = versionNumber },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersions(string targetKey, int beforeVersion, CancellationToken token = default)
    {
        var script = provider.GetScript(ScriptName.DeleteVersionsByNumber);

        return ExecuteSqliteScriptAsync(
            script,
            new { TargetKey = targetKey, VersionNumber = beforeVersion },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersions(string targetKey, DateTime beforeDate, CancellationToken token = default)
    {
        var script = provider.GetScript(ScriptName.DeleteVersionsBeforeDate);

        return ExecuteSqliteScriptAsync(
            script,
            new { TargetKey = targetKey, BeforeDate = beforeDate },
            token
        );
    }

    /// <inheritdoc />
    public Task PutImport(Import import, CancellationToken token = default)
    {
        var script = provider.GetScript(ScriptName.PutImport);

        return ExecuteSqliteScriptAsync(script, new
        {
            import.ImportId,
            ImportStatus = import.Status.ToString(),
            SourceType = import.SourceType.ToString(),
            FileType = import.FileType.ToString(),
            import.FileName
        }, token);
    }

    /// <inheritdoc />
    public Task LogImport(ImportLog log, CancellationToken token = default)
    {
        var script = provider.GetScript(ScriptName.PostLog);

        return ExecuteSqliteScriptAsync(script, new
        {
            log.ImportId,
            LogSeverity = log.LogSeverity.ToString(),
            log.LogMessage,
            log.LogException
        }, token);
    }

    /// <summary>
    /// Executes the specified SQL script asynchronously using the database session.
    /// </summary>
    private async Task ExecuteSqliteScriptAsync(string scriptName, object parameters, CancellationToken token)
    {
        await using var connection = await provider.OpenConnection(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await connection.ExecuteAsync(scriptName, parameters, transaction);
            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
}
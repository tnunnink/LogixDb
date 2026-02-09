using LogixDb.Core;

namespace LogixDb.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ILogixDatabase
{
    /// <summary>
    /// 
    /// </summary>
    void Migrate();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetKey"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<Snapshot>> Snapshots(string? targetKey = null, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetKey"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Snapshot> Import(string source, string? targetKey = null, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetKey"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Snapshot> Export(string targetKey, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="snapshotId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Snapshot> Export(int snapshotId, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Purge(CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetKey"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Purge(string targetKey, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="snapshotId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Purge(int snapshotId, CancellationToken token = default);
}
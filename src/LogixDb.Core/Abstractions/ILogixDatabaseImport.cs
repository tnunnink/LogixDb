using LogixDb.Core.Common;

namespace LogixDb.Core.Abstractions;

public interface ILogixDatabaseImport
{
    Task Process(Snapshot snapshot, CancellationToken token = default);
}
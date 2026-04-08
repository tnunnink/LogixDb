using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on programs.
/// </summary>
internal class ProgramTransformer : ISnapshotTransformer
{
    private readonly ProgramMap _map = new();
    private readonly Dictionary<string, ProgramRecord> _records = [];

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        _records.Clear();

        var source = snapshot.GetSource();

        foreach (var program in source.Programs)
        {
            GetOrAddRecord(snapshot.SnapshotId, program);
        }

        yield return _map.GenerateTable(_records.Values);
    }

    /// <summary>
    /// Retrieves an existing <see cref="ProgramRecord"/> from the cache if it exists or adds a new one to the cache
    /// after constructing it using the specified snapshot ID and program details.
    /// </summary>
    /// <param name="snapshotId">The identifier for the snapshot to which the program belongs.</param>
    /// <param name="program">The program for which the record needs to be retrieved or created.</param>
    /// <returns>A <see cref="ProgramRecord"/> that represents the program along with its relationships and metadata.</returns>
    private ProgramRecord GetOrAddRecord(int snapshotId, Program program)
    {
        // If already added, return the cached instance.
        if (_records.TryGetValue(program.Name, out var existing))
            return existing;

        // This assumes all tasks have been processed first.
        var taskId = program.Task?.Metadata.Get<Guid>("id");
        // Recursively get/add parent programs as needed.
        var parent = program.Parent is not null ? GetOrAddRecord(snapshotId, program.Parent) : null;

        // Use these relationships to build the program record.
        var record = new ProgramRecord(snapshotId, taskId, parent?.ProgramId, program);
        // Seed the program id we generated so that other transformers can reference this instance.
        program.Metadata.Add("id", record.ProgramId);

        // Cache and return the program record for reference and import.
        _records.Add(program.Name, record);
        return record;
    }
}
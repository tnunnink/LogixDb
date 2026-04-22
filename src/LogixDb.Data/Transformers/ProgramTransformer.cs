using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on programs.
/// </summary>
internal class ProgramTransformer : IDbTransformer
{
    private readonly ProgramMap _map = new();
    private readonly Dictionary<string, ProgramRecord> _records = [];

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        _records.Clear();

        var source = target.GetSource();

        foreach (var program in source.Programs)
        {
            GetOrAddRecord(target.InstanceId, program);
        }

        yield return _map.GenerateTable(_records.Values);
    }

    /// <summary>
    /// Retrieves an existing <see cref="ProgramRecord"/> from the cache if it exists or adds a new one to the cache
    /// after constructing it using the specified Target ID and program details.
    /// </summary>
    /// <param name="instanceId">The identifier for the Target to which the program belongs.</param>
    /// <param name="program">The program for which the record needs to be retrieved or created.</param>
    /// <returns>A <see cref="ProgramRecord"/> that represents the program along with its relationships and metadata.</returns>
    private ProgramRecord GetOrAddRecord(int instanceId, Program program)
    {
        // If already added, return the cached instance.
        if (_records.TryGetValue(program.Name, out var existing))
            return existing;

        // This assumes all tasks have been processed first.
        var taskId = program.Task?.Metadata.Get<Guid>("id");
        // Recursively get/add parent programs as needed.
        var parent = program.Parent is not null ? GetOrAddRecord(instanceId, program.Parent) : null;

        // Use these relationships to build the program record.
        var record = new ProgramRecord(instanceId, taskId, parent?.ProgramId, program);
        // Seed the program id we generated so that other transformers can reference this instance.
        program.Metadata.Add("id", record.ProgramId);

        // Cache and return the program record for reference and import.
        _records.Add(program.Name, record);
        return record;
    }
}

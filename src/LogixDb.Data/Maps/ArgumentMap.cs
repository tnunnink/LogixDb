using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the <see cref="ArgumentRecord"/> entity,
/// defining the structure and behavior for database interactions with the "argument" table.
/// </summary>
/// <remarks>
/// This class specifies the table name and provides a collection of column mappings
/// that associate fields in the <see cref="ArgumentRecord"/> class with corresponding
/// database table columns. It is used to define how data is serialized and deserialized
/// between domain objects and database records.
/// </remarks>
internal class ArgumentMap : TableMap<ArgumentRecord>
{
    /// <inheritdoc />
    protected override string TableName => "argument";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ArgumentRecord>> Columns =>
    [
        ColumnMap<ArgumentRecord>.For(r => r.ArgumentId, "argument_id"),
        ColumnMap<ArgumentRecord>.For(r => r.InstructionId, "instruction_id"),
        ColumnMap<ArgumentRecord>.For(r => r.Index, "argument_index"),
        ColumnMap<ArgumentRecord>.For(r => r.Argument.Type, "argument_type"),
        ColumnMap<ArgumentRecord>.For(r => r.Argument.ToString(), "argument_text")
    ];
}

/// <summary>
/// Represents a database record for an instruction argument within a snapshot.
/// </summary>
/// <remarks>
/// This record encapsulates the data necessary to store and retrieve instruction arguments
/// from the database, including their relationship to a snapshot and parent instruction,
/// as well as their position and content.
/// </remarks>
internal record ArgumentRecord(Guid InstructionId, byte Index, Argument Argument)
{
    public Guid ArgumentId { get; } = Guid.NewGuid();
}
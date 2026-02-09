using LogixDb.Core;

namespace LogixDb.Abstractions;

/// <summary>
/// Represents a provider that resolves instances of <see cref="ILogixDatabase"/> based on a given <see cref="SqlProvider"/>.
/// </summary>
public interface ILogixDatabaseProvider
{
    /// <summary>
    /// Resolves an instance of <see cref="ILogixDatabase"/> for the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key specifying the type of SQL provider to resolve.</param>
    /// <returns>An instance of <see cref="ILogixDatabase"/> corresponding to the provided key.</returns>
    ILogixDatabase Resolve(SqlProvider key);
}
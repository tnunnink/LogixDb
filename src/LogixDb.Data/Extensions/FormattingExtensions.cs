using L5Sharp.Core;

namespace LogixDb.Data.Extensions;

/// <summary>
/// Provides extension methods for formatting and extracting information from Logix elements,
/// such as data type names, bit numbers, and SQL-compatible string formats.
/// </summary>
public static class FormattingExtensions
{
    /// <summary>
    /// Retrieves the data type name of the provided Logix element.
    /// For tags and parameters with dimensions, the dimensional index is appended to the data type name.
    /// </summary>
    /// <param name="element"></param>
    /// <returns>A string representing the data type name, including dimensional information if applicable.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the element type is unsupported for data type name extraction.
    /// </exception>
    internal static string GetDataTypeName(this ILogixElement element)
    {
        return element switch
        {
            Tag t => t.Dimensions > 0 ? $"{t.DataType}{t.Dimensions.ToIndex()}" : t.DataType,
            Parameter p => p.Dimensions?.IsEmpty is false ? $"{p.DataType}{p.Dimensions.ToIndex()}" : p.DataType,
            _ => throw new InvalidOperationException(
                $"Element type '{element.GetType().Name}' is not supported for data type name extraction.")
        };
    }

    /// <summary>
    /// Retrieves the bit number associated with a given <c>DataTypeMember</c>, if available.
    /// </summary>
    /// <param name="member">The <c>DataTypeMember</c> from which the bit number is to be obtained.</param>
    /// <returns>The bit number as a byte if it is not null; otherwise, null.</returns>
    internal static byte? GetBitNumber(this DataTypeMember member)
    {
        return member.BitNumber is not null ? (byte)member.BitNumber : null;
    }

    /// <summary>
    /// Extracts the string representation of the value from the provided Logix data element.
    /// Only atomic data types (such as DINT, REAL, BOOL) are supported for value extraction.
    /// </summary>
    /// <param name="data">The LogixData element from which to extract the value.</param>
    /// <returns>
    /// A string representation of the atomic data value if the data is atomic; otherwise, null
    /// for complex or structured data types.
    /// </returns>
    internal static string? ToSqlFormat(this LogixData data)
    {
        return data switch
        {
            AtomicData atomic => atomic.ToString(),
            _ => null
        };
    }

    /// <summary>
    /// Converts a Dimensions object to its SQL-compatible string format.
    /// If the Dimensions object is empty, it converts it to its index representation; otherwise, it returns null.
    /// </summary>
    /// <param name="dimensions">The Dimensions object to be converted.</param>
    /// <returns>A SQL-compatible string representation of the Dimensions object, or null if the object is not empty.</returns>
    internal static string? ToSqlFormat(this Dimensions? dimensions)
    {
        return dimensions?.IsEmpty is false ? dimensions.ToIndex() : null;
    }

    /// <summary>
    /// Converts a <see cref="Radix"/> object to its SQL-compatible string representation if applicable.
    /// </summary>
    /// <param name="radix">The <see cref="Radix"/> object to convert. Can be null.</param>
    /// <returns>The SQL-compatible string representation of the <paramref name="radix"/> name, or null if the input is null or represents no value.</returns>
    internal static string? ToSqlFormat(this Radix? radix)
    {
        return radix is not null && radix != Radix.Null ? radix.Name : null;
    }
}
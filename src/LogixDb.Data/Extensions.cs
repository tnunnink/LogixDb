using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using L5Sharp.Core;

namespace LogixDb.Data;

/// <summary>
/// Provides extension methods for string and byte array manipulation, including compression,
/// decompression, and hashing operations used throughout the LogixDb system.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Compresses a string into a byte array using GZip compression.
    /// The string is first encoded using Unicode encoding before compression.
    /// </summary>
    /// <param name="text">The text string to compress.</param>
    /// <returns>A compressed byte array representation of the input text.</returns>
    internal static byte[] Compress(this string text)
    {
        var bytes = Encoding.Unicode.GetBytes(text);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        {
            msi.CopyTo(gs);
        }

        return mso.ToArray();
    }

    /// <summary>
    /// Decompresses a byte array back into a string using GZip decompression.
    /// The decompressed bytes are decoded using Unicode encoding to reconstruct the original string.
    /// </summary>
    /// <param name="bytes">The compressed byte array to decompress.</param>
    /// <returns>The decompressed string representation of the input bytes.</returns>
    internal static string Decompress(this byte[] bytes)
    {
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        {
            gs.CopyTo(mso);
        }

        return Encoding.Unicode.GetString(mso.ToArray());
    }

    /// <summary>
    /// Computes the MD5 hash of the input text.
    /// The input string is encoded using UTF-8 before generating the hash.
    /// </summary>
    /// <param name="text">The input string to hash.</param>
    /// <returns>A byte array representing the computed MD5 hash of the input text.</returns>
    internal static byte[] Hash(this string text)
    {
        return MD5.HashData(Encoding.Unicode.GetBytes(text));
    }

    /// <summary>
    /// Converts a byte array to its lowercase hexadecimal string representation.
    /// </summary>
    /// <param name="binary">The byte array to be converted.</param>
    /// <returns>A string containing the lowercase hexadecimal representation of the input byte array.</returns>
    internal static string ToHexString(this byte[] binary)
    {
        return Convert.ToHexStringLower(binary);
    }

    /// <param name="element"></param>
    extension(ILogixElement element)
    {
        /// <summary>
        /// Serializes and compresses the Logix element into a byte array using GZip compression.
        /// The element is first serialized to XML format, then compressed to reduce storage size.
        /// </summary>
        /// <returns></returns>
        internal byte[] Compress()
        {
            return element.Serialize().ToString().Compress();
        }

        /// <summary>
        /// Computes the hash of a serialized Logix element and returns its lowercase hexadecimal string representation.
        /// The Logix element is serialized to XML format before hashing using MD5 and
        /// converting the result to a hexadecimal string.
        /// </summary>
        /// <returns>A string representing the MD5 hash of the serialized Logix element in lowercase hexadecimal format.</returns>
        internal string Hash()
        {
            return element.Serialize().ToString().Hash().ToHexString();
        }

        /// <summary>
        /// Retrieves the data type name of the provided Logix element.
        /// For tags and parameters with dimensions, the dimensional index is appended to the data type name.
        /// </summary>
        /// <returns>A string representing the data type name, including dimensional information if applicable.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the element type is unsupported for data type name extraction.
        /// </exception>
        internal string GetDataTypeName()
        {
            return element switch
            {
                Tag t => t.Dimensions > 0 ? $"{t.DataType}{t.Dimensions.ToIndex()}" : t.DataType,
                Parameter p => p.Dimension > 0 ? $"{p.DataType}{p.Dimension.ToIndex()}" : p.DataType,
                _ => throw new InvalidOperationException(
                    $"Element type '{element.GetType().Name}' is not supported for data type name extraction.")
            };
        }
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
    internal static string? GetDataValue(this LogixData data)
    {
        return data switch
        {
            AtomicData atomic => atomic.ToString(),
            _ => null
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
    /// Converts a Dimensions object to its SQL-compatible string format.
    /// If the Dimensions object is empty, it converts it to its index representation; otherwise, returns null.
    /// </summary>
    /// <param name="dimensions">The Dimensions object to be converted.</param>
    /// <returns>A SQL-compatible string representation of the Dimensions object, or null if the object is not empty.</returns>
    internal static string? ToSqlFormat(this Dimensions? dimensions)
    {
        return dimensions?.IsEmpty is true ? dimensions.ToIndex() : null;
    }

    /// <summary>
    /// Converts the specified dimensions object into its SQL-compatible string format.
    /// If the dimensions object is null, returns null.
    /// </summary>
    /// <param name="dimensions">The dimensions object to convert.</param>
    /// <returns>A SQL-compatible string representation of the dimensions, or null if the input is null.</returns>
    internal static string? ToSqlFormat(this Radix? radix)
    {
        return radix is not null && radix != Radix.Null ? radix.Name : null;
    }
}
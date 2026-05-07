using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
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
        /// Computes a standardized hash representation of the Logix element.
        /// The element is scrubbed to remove sensitive or unnecessary data, serialized into XML format,
        /// and then hashed using MD5, with the resulting hash returned as a lowercase hexadecimal string.
        /// </summary>
        /// <returns>A lowercase hexadecimal string representing the hash of the scrubbed and serialized Logix element.</returns>
        public string Hash()
        {
            return element.ScrubData().Serialize().ToString(SaveOptions.DisableFormatting).Hash().ToHexString();
        }

        /// <summary>
        /// Removes or anonymizes sensitive and non-essential data from a Logix element to standardize
        /// it for comparison or hashing purposes. This includes clearing or removing specific attributes and elements,
        /// such as values, formats, and communication-related data.
        /// </summary>
        /// <returns>The scrubbed Logix element with sensitive or unnecessary data removed.</returns>
        private ILogixElement ScrubData()
        {
            foreach (var d in element.Serialize().DescendantsAndSelf())
            {
                switch (d.Name.LocalName)
                {
                    // Remove 'Value' attributes which contain the actual data for Atomic tags/members
                    case L5XName.DataValue or L5XName.DataValueMember or L5XName.Element:
                        d.Attributes(L5XName.Value).Remove();
                        break;
                    case L5XName.Data:
                    {
                        var format = d.Attribute(L5XName.Format)?.Value;

                        // If it's a L5K/String format, scrub the element's value/text.
                        if (format == DataFormat.L5K || format == DataFormat.String)
                        {
                            d.RemoveNodes();
                        }
                        // For special formats (Alarms/Message parameters), clear child attributes/values but keep structure
                        else if (format != DataFormat.Decorated)
                        {
                            d.Elements()
                                .SelectMany(x => x.Attributes())
                                .ToList()
                                .ForEach(a => a.SetValue(string.Empty));
                        }

                        break;
                    }
                    // Remove all module communication elements. These contain IO tags and other Hex data not stored in the database.
                    case L5XName.Communications:
                        d.Remove();
                        break;
                }
            }

            return element;
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
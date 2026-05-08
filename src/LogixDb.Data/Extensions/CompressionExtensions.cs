using System.IO.Compression;
using System.Text;

namespace LogixDb.Data.Extensions;

/// <summary>
/// Provides extension methods for compressing and decompressing data.
/// </summary>
public static class CompressionExtensions
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
}
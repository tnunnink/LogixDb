using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace LogixDb.Core;

/// <summary>
/// 
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string Compress(this string text)
    {
        var bytes = Encoding.Unicode.GetBytes(text);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        {
            msi.CopyTo(gs);
        }

        return Convert.ToBase64String(mso.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    internal static string Decompress(this string data)
    {
        var bytes = Convert.FromBase64String(data);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        {
            gs.CopyTo(mso);
        }

        return Encoding.Unicode.GetString(mso.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Hash(this string text)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexStringLower(hash);
    }
}
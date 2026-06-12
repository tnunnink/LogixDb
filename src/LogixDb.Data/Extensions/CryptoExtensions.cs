using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using L5Sharp.Core;

namespace LogixDb.Data.Extensions;

/// <summary>
/// Provides cryptographic utility methods for generating SHA-256 hashes from strings and objects.
/// </summary>
public static class CryptoExtensions
{
    /// <summary>
    /// Computes an SHA-256 hash of the provided string and returns it as a lowercase hexadecimal string.
    /// The string is encoded using Unicode (UTF-16) encoding before hashing.
    /// </summary>
    /// <param name="text">The string to hash.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA-256 hash of the input text.</returns>
    public static string HashText(this string text)
    {
        var bytes = Encoding.Unicode.GetBytes(text);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>
    /// Computes an SHA-256 hash of the canonicalized version of the specified XML element and returns it as a lowercase hexadecimal string.
    /// The canonicalization process transforms the XML element into its canonical form using the "Exclusive XML Canonicalization" algorithm (C14N)
    /// before generating the hash.
    /// </summary>
    /// <param name="element">The XML element to canonicalize and hash.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA-256 hash of the canonicalized XML element.</returns>
    public static string HashElement(this ILogixElement element)
    {
        // Return cached hash if found for the provided element.
        if (element.Metadata.TryGetValue("content_hash", out var cached))
            return (string)cached;

        // Always clone the element to prevent mutation from affecting the import.
        var clone = element.Clone();

        // This will recursively reset all data members to default values.
        // We need to do this to ensure the hash does not reflect data value changes.
        // We als rely on the fact that Target is scrubbing the L5K data when loaded/created.
        switch (clone)
        {
            case Tag tag:
                tag.Value.ClearData();
                break;
            case Parameter parameter:
                parameter.Default?.ClearData();
                break;
            case LogixData data:
                data.ClearData();
                break;
        }

        // 2. Convert XElement to XmlDocument (C14N works on XmlDocument)
        var document = new XmlDocument();
        using (var reader = clone.Serialize().CreateReader())
        {
            document.Load(reader);
        }

        // 3. Apply the Canonicalization Transform
        var transform = new XmlDsigC14NTransform();
        transform.LoadInput(document);

        using var output = (Stream)transform.GetOutput();
        using var memory = new MemoryStream();
        output.CopyTo(memory);

        // 4. Hash the resulting canonical bytes
        var bytes = memory.ToArray();
        var hash = Convert.ToHexStringLower(SHA256.HashData(bytes));

        // 5. Cache for future use to avoid expensive recomputation.
        element.Metadata["content_hash"] = hash;
        return hash;
    }
}
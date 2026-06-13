using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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

        // We need to clean volatile data from an element to ensure a proper "config" hash.
        // For tags that means scrubbing data values, which we can easily do with the clear data method.
        // For a module, we want to just blow away tag data since it is handled in the tag table.
        // Note that all L5K data is already scrubbed at this point, so things like ConfigScript should be fine.
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
            case Module module:
                module.Serialize().Descendants().Where(e => e.Name.LocalName.Contains(L5XName.Tag)).Remove();   
                break;
        }

        // Convert XElement to XmlDocument (C14N works on XmlDocument)
        var document = new XmlDocument();
        using (var reader = clone.Serialize().CreateReader())
        {
            document.Load(reader);
        }

        // Apply the Canonicalization Transform
        var transform = new XmlDsigC14NTransform();
        transform.LoadInput(document);

        using var output = (Stream)transform.GetOutput();
        using var memory = new MemoryStream();
        output.CopyTo(memory);

        // Hash the resulting canonical bytes
        var bytes = memory.ToArray();
        var hash = Convert.ToHexStringLower(SHA256.HashData(bytes));

        // Cache for future use to avoid expensive recomputation.
        element.Metadata["content_hash"] = hash;
        return hash;
    }
}
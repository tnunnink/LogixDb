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
        // 0. Return cached hash if found for the provided element.
        if (element.Metadata.TryGetValue("content_hash", out var cached))
            return (string)cached;

        // 1. Scrub any volatile data values from the element if found.
        var scrubbed = ScrubDataValues(element.Serialize());

        // 2. Convert XElement to XmlDocument (C14N works on XmlDocument)
        var document = new XmlDocument();
        using (var reader = scrubbed.CreateReader())
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

    /// <summary>
    /// Removes specific data values or attributes from the given XElement, ensuring that volatile or sensitive
    /// information is scrubbed while retaining structural integrity where necessary.
    /// </summary>
    /// <param name="element">The XElement to scrub volatile or sensitive data values from.</param>
    /// <returns>A new XElement with the specified data values or attributes removed or modified as per defined rules.</returns>
    private static XElement ScrubDataValues(XElement element)
    {
        var clone = new XElement(element);

        foreach (var descendant in clone.DescendantsAndSelf())
        {
            switch (descendant.Name.LocalName)
            {
                // Remove 'Value' attributes which contain the actual data for Atomic tags/members
                case L5XName.DataValue or L5XName.DataValueMember or L5XName.Element:
                    descendant.Attributes(L5XName.Value).Remove();
                    break;
                case L5XName.Data:
                {
                    var format = descendant.Attribute(L5XName.Format)?.Value;

                    // If it's a L5K/String format, scrub the element's value/text.
                    if (format == DataFormat.L5K || format == DataFormat.String)
                    {
                        descendant.RemoveNodes();
                    }
                    // For special formats (e.g., Alarms/Message parameters), clear child attributes but keep structure
                    else if (format != DataFormat.Decorated)
                    {
                        descendant.Elements()
                            .SelectMany(x => x.Attributes())
                            .ToList()
                            .ForEach(a => a.SetValue(string.Empty));
                    }

                    break;
                }
            }
        }

        return clone;
    }
}
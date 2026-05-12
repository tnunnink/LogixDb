/*using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using L5Sharp.Core;
using Task = L5Sharp.Core.Task;

namespace LogixDb.Data;

/// <summary>
/// A utility class for hashing various Logix design elements, providing a mechanism to generate consistent,
/// type-specific hashes for use in comparisons, database storage, or versioning.
/// </summary>
public static class ElementHasher
{
    /// <summary>
    /// Represents a collection of controller-specific elements used for hashing purposes.
    /// This set includes key metadata fields that are relevant to controllers in a Logix system,
    /// such as description, redundancy information, security settings, and safety information.
    /// </summary>
    private static readonly HashSet<string> ControllerElements =
    [
        L5XName.Description, L5XName.RedundancyInfo, L5XName.Security, L5XName.SafetyInfo
    ];

    /// <summary>
    /// Represents a set of module-specific elements used for hashing and comparison purposes.
    /// This collection includes critical metadata fields pertinent to Logix modules, such as
    /// descriptive information, electronic key settings, and port configurations.
    /// </summary>
    private static readonly HashSet<string> ModuleElements =
    [
        L5XName.Description, L5XName.EKey, L5XName.Ports
    ];

    /// <summary>
    /// Represents a collection of task-specific elements used for hashing purposes.
    /// This set includes metadata fields relevant to tasks within a Logix system,
    /// such as description and event-related information.
    /// </summary>
    private static readonly HashSet<string> TaskElements =
    [
        L5XName.Description, L5XName.EventInfo
    ];

    /// <summary>
    /// Generates a hash for a given Logix element by processing its serialized representation. The method dynamically determines
    /// the specific type of the element and applies a type-specific hashing approach. If the hash has been computed previously
    /// and cached in the element's metadata, the cached value is returned.
    /// </summary>
    /// <param name="element">The Logix element to be hashed. This can be any element that implements the <see cref="ILogixElement"/> interface.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the element, or null if the input element is null.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a hash cannot be computed for the provided element type.</exception>
    public static string? Hash(this ILogixElement? element)
    {
        if (element is null) return null;

        if (element.Metadata.TryGetValue("hash", out var cached) && cached is string hex)
            return hex;

        var hash = element switch
        {
            Controller controller => ShallowHash(controller, ControllerElements),
            DataType dataType => ShallowHash(dataType, [L5XName.Description]),
            AddOnInstruction aoi => ShallowHash(aoi, [L5XName.Description]),
            Module module => ShallowHash(module, ModuleElements),
            Task task => ShallowHash(task, TaskElements),
            Program program => ShallowHash(program, [L5XName.Description]),
            Routine routine => ShallowHash(routine, [L5XName.Description]),
            Tag tag => ShallowHash(tag),
            LogixData data => ShallowHash(ScrubDataValues(data)),
            _ => DeepHash(element)
        };

        element.Metadata.Add("hash", hash);
        return hash;
    }

    /// <summary>
    /// Computes a shallow hash for a given Logix element by cloning and serializing its structure, excluding specified
    /// sub-elements based on a provided set of inclusion criteria. This method is designed to generate a lightweight hash
    /// where only selected parts of the element are included in the computation.
    /// </summary>
    /// <param name="element">
    /// The Logix element for which to compute the shallow hash. The element must implement
    /// the <see cref="ILogixElement"/> interface.
    /// </param>
    /// <param name="include">
    /// An optional set of element names to include in the hash computation. Any sub-elements whose names are not in this set
    /// will be excluded from the serialized representation of the element.
    /// </param>
    /// <returns>A lowercase hexadecimal string representing the shallow hash of the specified element.</returns>
    private static string ShallowHash(ILogixElement element, HashSet<string>? include = null)
    {
        var xml = element.Serialize();

        // If we don't want any children, create a new XElement with just the root's attributes.
        if (include is null || include.Count == 0)
        {
            var root = new XElement(xml.Name, xml.Attributes());
            return ComputeHash(root);
        }

        // Otherwise, clone and filter children
        var clone = new XElement(xml);
        clone.Elements().Where(e => !include.Contains(e.Name.LocalName)).Remove();
        return ComputeHash(clone);
    }

    /// <summary>
    /// Generates a deep hash for a given Logix element by processing its serialized representation and excluding specified child elements.
    /// This method ensures that the resulting hash reflects the structure and data of the element except for the excluded parts,
    /// making it suitable for deep comparisons and versioning.
    /// </summary>
    /// <param name="element">The Logix element for which the deep hash will be computed. This can be any element that implements the <see cref="ILogixElement"/> interface.</param>
    /// <param name="exclude">A collection of child elements to exclude from the hash computation. If null or empty, all child elements will be included in the hash.</param>
    /// <returns>A lowercase hexadecimal string representing the computed hash of the element, considering any exclusions. Returns null if the input element is null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided element is null.</exception>
    private static string DeepHash(ILogixElement element, HashSet<string>? exclude = null)
    {
        var xml = element.Serialize();

        // If we want all children, just return the complete hash of the provided element.
        if (exclude is null || exclude.Count == 0)
        {
            return ComputeHash(xml);
        }

        // Otherwise, clone and filter children
        var clone = new XElement(xml);
        clone.Elements().Where(e => exclude.Contains(e.Name.LocalName)).Remove();
        return ComputeHash(clone);
    }

    /// <summary>
    /// Removes sensitive or unnecessary data value attributes from a given <see cref="LogixData"/> object.
    /// This method creates a sanitized clone of the input data, preserving its structure while clearing
    /// or modifying specific values based on the type and format of the data.
    /// </summary>
    /// <param name="data">The <see cref="LogixData"/> instance to be scrubbed, typically containing serialized design data.</param>
    /// <returns>A sanitized <see cref="LogixData"/> instance with sensitive or unnecessary value attributes removed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the input data contains an unsupported format or cannot be serialized/deserialized.</exception>
    private static LogixData ScrubDataValues(LogixData data)
    {
        var clone = new XElement(data.Serialize());

        switch (clone.Name.LocalName)
        {
            // Remove 'Value' attributes which contain the actual data for Atomic tags/members
            case L5XName.DataValue or L5XName.DataValueMember or L5XName.Element:
                clone.Attributes(L5XName.Value).Remove();
                break;
            case L5XName.Data:
            {
                var format = clone.Attribute(L5XName.Format)?.Value;

                // If it's a L5K/String format, scrub the element's value/text.
                if (format == DataFormat.L5K || format == DataFormat.String)
                {
                    clone.RemoveNodes();
                }
                // For special formats (e.g., Alarms/Message parameters), clear child attributes but keep structure
                else if (format != DataFormat.Decorated)
                {
                    clone.Elements()
                        .SelectMany(x => x.Attributes())
                        .ToList()
                        .ForEach(a => a.SetValue(string.Empty));
                }

                break;
            }
        }

        return clone.Deserialize<LogixData>();
    }

    /// <summary>
    /// Generates a hash for the provided XML element by first converting it to a string representation with formatting disabled,
    /// encoding it to bytes using Unicode, and then applying the SHA256 hashing algorithm.
    /// The resulting hash is returned as a lowercase hexadecimal string.
    /// </summary>
    /// <param name="element">The XML element to be hashed. This should represent a scrubbed and serialized Logix element.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA256 hash of the input XML element.</returns>
    private static string ComputeHash(XElement element)
    {
        var text = element.ToString(SaveOptions.DisableFormatting);
        var bytes = Encoding.Unicode.GetBytes(text);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash);
    }
}*/
using System.Security.Cryptography;
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
    /// Computes a hash for the given Logix element based on its type, applying a specific hashing strategy tailored to
    /// the element type. The method serializes the element, optionally processes it, and generates a hash as a lowercase
    /// hexadecimal string. If the element type is not supported, an exception is thrown.
    /// </summary>
    /// <param name="element">The Logix element to hash. The processing and hashing strategy vary depending on the element type.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the processed Logix element.</returns>
    /// <exception cref="NotSupportedException">Thrown if the hashing method does not support the provided element type.</exception>
    public static string Hash(ILogixElement element)
    {
        return element switch
        {
            Controller controller => HashController(controller),
            DataType dataType => HashDataType(dataType),
            DataTypeMember member => HashElement(member.Serialize()),
            Module module => HashModule(module),
            Task task => HashTask(task),
            Program program => HashProgram(program),
            Routine routine => HashRoutine(routine),
            Rung rung => HashElement(rung.Serialize()),
            Tag tag => HashElement(ScrubData(tag.Serialize())),
            _ => throw new NotSupportedException(
                $"Hashing is not supported for element type '{element.GetType().Name}'.")
        };
    }

    /// <summary>
    /// Generates a hash for a given Logix controller element by processing its XML representation.
    /// Non-essential details, such as description, redundancy information, security, and safety information,
    /// are removed from the XML before hashing to ensure only the core attributes of the controller are considered.
    /// </summary>
    /// <param name="element">The controller element to be hashed. This should be a Logix Controller that can be serialized into an XML representation.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the processed XML representation of the controller.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the controller element is null.</exception>
    private static string HashController(Controller element)
    {
        var xml = element.Clone().Serialize();

        xml.Elements().Where(e => e.Name.LocalName
            is not L5XName.Description
            and not L5XName.RedundancyInfo
            and not L5XName.Security
            and not L5XName.SafetyInfo
        ).Remove();

        return HashElement(xml);
    }

    /// <summary>
    /// Generates a hash for the provided Logix <see cref="DataType"/> instance by cloning and filtering its serialized XML representation.
    /// The method removes all elements except the description and processes the remaining XML to produce a consistent hash value.
    /// </summary>
    /// <param name="element">The <see cref="DataType"/> instance to hash. The hash is based on a scrubbed version of its serialized representation.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA256 hash of the processed <see cref="DataType"/>.</returns>
    private static string HashDataType(DataType element)
    {
        var xml = element.Clone().Serialize();
        xml.Elements().Where(e => e.Name.LocalName is not L5XName.Description).Remove();
        return HashElement(xml);
    }

    /// <summary>
    /// Generates a hash for a given Logix module. This process retains only specific sub-elements
    /// such as Description, EKey, and Ports while removing others, thereby producing a consistent
    /// and manageable XML representation for hashing. The resulting standardized XML is then hashed
    /// into a lowercase hexadecimal string.
    /// </summary>
    /// <param name="element">The Logix module to hash. Only certain sub-elements of the module, such as Description, EKey, and Ports, are included in the hashing process.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the processed Logix module's XML representation.</returns>
    private static string HashModule(Module element)
    {
        var xml = element.Clone().Serialize();

        // For a module the only sub elements we pull are description, EKey, and Ports.
        // Tag data is handled by tag imports
        xml.Elements().Where(e => e.Name.LocalName
                is not L5XName.Description
                and not L5XName.EKey
                and not L5XName.Ports)
            .Remove();

        return HashElement(xml);
    }

    /// <summary>
    /// Computes a hash for the specified Logix task element, applying a hashing strategy that focuses only on
    /// specific sub-elements such as description and event information. The method filters the task's XML
    /// representation to retain only the relevant parts before creating the hash.
    /// </summary>
    /// <param name="element">The Logix task element to be hashed. Only its description and event-related
    /// sub-elements are considered for the hash computation.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the processed task element.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided task element is null.</exception>
    private static string HashTask(Task element)
    {
        var xml = element.Clone().Serialize();

        // For a task the only sub elements we pull are description and event info
        xml.Elements()
            .Where(e => e.Name.LocalName is not L5XName.Description and not L5XName.EventInfo)
            .Remove();

        return HashElement(xml);
    }

    /// <summary>
    /// Generates a hash for the given Logix Program element by serializing it into an XML format,
    /// removing specific elements such as descriptions, and then computing the hash. This method
    /// ensures that only relevant data contributes to the hash value, providing a consistent and
    /// reliable program hash.
    /// </summary>
    /// <param name="element">The Program element for which the hash will be generated.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the processed Program element.</returns>
    private static string HashProgram(Program element)
    {
        var xml = element.Clone().Serialize();
        xml.Elements().Where(e => e.Name.LocalName is not L5XName.Description).Remove();
        return HashElement(xml);
    }

    /// <summary>
    /// Computes a hash for the provided Logix routine by cloning and serializing the routine, removing all non-essential
    /// elements such as descriptions, and then applying a specialized hashing strategy. This ensures that only fundamental
    /// structure and data contribute to the hash, making it consistent and reliable for comparison or storage.
    /// </summary>
    /// <param name="element">The Logix routine to hash.</param>
    /// <returns>A lowercase hexadecimal string representing the hash of the processed Logix routine.</returns>
    private static string HashRoutine(Routine element)
    {
        var xml = element.Clone().Serialize();
        xml.Elements().Where(e => e.Name.LocalName is not L5XName.Description).Remove();
        return HashElement(xml);
    }

    /// <summary>
    /// Processes and removes sensitive or unnecessary data from the provided Logix element,
    /// modifying its structure and attributes according to predefined rules.
    /// </summary>
    /// <param name="element">The Logix element to be scrubbed of sensitive or irrelevant data.</param>
    /// <returns>The modified Logix element after applying the data scrubbing logic.</returns>
    private static XElement ScrubData(XElement element)
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

    /// <summary>
    /// Generates a hash for the provided XML element by first converting it to a string representation with formatting disabled,
    /// encoding it to bytes using Unicode, and then applying the SHA256 hashing algorithm.
    /// The resulting hash is returned as a lowercase hexadecimal string.
    /// </summary>
    /// <param name="element">The XML element to be hashed. This should represent a scrubbed and serialized Logix element.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA256 hash of the input XML element.</returns>
    private static string HashElement(XElement element)
    {
        var text = element.ToString(SaveOptions.DisableFormatting);
        var bytes = Encoding.Unicode.GetBytes(text);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash);
    }
}
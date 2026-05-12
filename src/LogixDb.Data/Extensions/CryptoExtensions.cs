using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace LogixDb.Data.Extensions;

/// <summary>
/// Provides cryptographic utility methods for generating SHA-256 hashes from strings and objects.
/// </summary>
public static class CryptoExtensions
{
    /// <summary>
    /// A thread-safe dictionary that maps a type to a list of tuples, where each tuple contains
    /// a property name as a string and a function to retrieve the property's value from an object of that type.
    /// Used internally to optimize property access and serialization logic in cryptographic hashing functions.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, List<PropertyMapping>> Mappings = [];

    /// <summary>
    /// Computes an SHA-256 hash of the provided string and returns it as a lowercase hexadecimal string.
    /// The string is encoded using Unicode (UTF-16) encoding before hashing.
    /// </summary>
    /// <param name="text">The string to hash.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA-256 hash of the input text.</returns>
    public static string Hash(this string text)
    {
        var bytes = Encoding.Unicode.GetBytes(text);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>
    /// Computes an SHA-256 hash of an object's properties, ordered by property name, and returns the hash as a
    /// lowercase hex string. The hash is based on the serialized representation of property names and their values.
    /// </summary>
    /// <param name="record">The object whose properties will be hashed. All public instance properties are used in the hash computation.</param>
    /// <param name="ignore">An optional set of property names to exclude from the hash computation. If null or empty, all properties are included.</param>
    /// <returns>A lowercase hexadecimal string representing the SHA-256 hash of the object's properties.</returns>
    public static string Hash<T>(this T record, HashSet<string>? ignore = null) where T : class
    {
        var properties = Mappings.GetOrAdd(typeof(T), GeneratePropertyMapping);

        var builder = new StringBuilder();

        foreach (var property in properties)
        {
            if (ignore?.Contains(property.Name) is true) continue;
            var value = property.Getter(record);
            builder.Append(SerializeField(property.Name, value));
        }

        return builder.ToString().Hash();
    }

    /// <summary>
    /// Serializes a field name and its value into a string format using specific delimiters.
    /// This format is used for hashing and preserving data consistency.
    /// </summary>
    /// <param name="name">The name of the field being serialized.</param>
    /// <param name="value">The value of the field to serialize. This can be a primitive type, string, or object.</param>
    /// <returns>A serialized string representation of the field name and value, delimited by special characters.</returns>
    private static string SerializeField(string name, object? value)
    {
        return '\u001E' + name + '\u001F' + FormatValue(value);

        static string FormatValue(object? value)
        {
            return value switch
            {
                null or DBNull => "\u2400",
                byte[] b => Convert.ToHexStringLower(b),
                string s => s.Replace("\r\n", "\n"),
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }
    }

    /// <summary>
    /// Generates a list of property mappings for the provided type. Each mapping associates a property name
    /// with a function that retrieves the property's value from an object instance of the specified type.
    /// </summary>
    /// <param name="type">The type for which property mappings are to be generated.</param>
    /// <returns>A list of tuples, where each tuple contains a property name and a function to retrieve its value.</returns>
    private static List<PropertyMapping> GeneratePropertyMapping(Type type)
    {
        var mappings = new List<PropertyMapping>();

        var parameter = Expression.Parameter(typeof(object), "x");
        var casted = Expression.Convert(parameter, type);

        // Order by Name here once, so we don't have to do it in every Hash call
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetIndexParameters().Length == 0)
            .OrderBy(p => p.Name);

        foreach (var propertyInfo in properties)
        {
            var property = Expression.Property(casted, propertyInfo);
            var boxed = Expression.Convert(property, typeof(object));
            var function = Expression.Lambda<Func<object, object?>>(boxed, parameter).Compile();
            mappings.Add(new PropertyMapping(propertyInfo.Name, function));
        }

        return mappings;
    }

    /// <summary>
    /// Represents a mapping between a property name and a function that retrieves the value of the property from an object.
    /// This is primarily used to facilitate efficient access to object properties and serialization logic.
    /// </summary>
    private readonly record struct PropertyMapping(string Name, Func<object, object?> Getter);
}
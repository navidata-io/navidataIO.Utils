// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace navidataIO.Utils.Json;

/// <summary>
/// Extension methods for working with JSON tokens.
/// </summary>
public static class JsonExtensions
{

    /// <summary>
    /// Removes the named property containing an array from the Json object, and returns the array elements as an <see cref="IEnumerable{T}"/>.
    /// Returns an empty enumeration if the property does not exist, but it will fail if the property does not contain an array.
    /// Any array elements that are not of type <typeparamref name="T"/> will be skipped.
    /// </summary>
    public static IEnumerable<T> RemoveArrayAs<T>(this JObject parent, string property)
    {
        if (parent == null) throw new ArgumentNullException(nameof(parent));
        if (string.IsNullOrEmpty(property))
            throw new ArgumentException("Value cannot be null or empty.", nameof(property));

        var array = parent[property]?.Value<JArray>();
        parent.Remove(property);

        return array != null ? array.OfType<T>() : Array.Empty<T>();
    }

    /// <summary>
    /// Adds a new property to the json object, with the string-encoded value of the provided token as the value.
    /// An existing property with the same name will be replaced. 
    /// </summary>
    /// <returns>The original json object, with the new property added.</returns>
    public static JObject InsertTokenAsString<T>(this JObject parent, string property, T? token) where T : JToken
    {
        if (parent == null) throw new ArgumentNullException(nameof(parent));
        if (string.IsNullOrEmpty(property))
            throw new ArgumentException("Value cannot be null or empty.", nameof(property));

        if (token != null)
            parent[property] = token.ToString(formatting: Newtonsoft.Json.Formatting.None);
        return parent;
    }

    /// <summary>
    /// Attempts to convert the specified JSON property to a value of type <typeparamref name="T"/>.
    /// Returns the converted value, or the default value if the property does not exist or cannot be converted.
    /// </summary>
    public static T? ReadPropertySafe<T>(this JObject json, string property, T? defaultValue = default, ILogger? logger = default)
    { 
        if (json == null) throw new ArgumentNullException(nameof(json));
        if (string.IsNullOrEmpty(property))
            throw new ArgumentException("Value cannot be null or empty.", nameof(property));

        if (json.TryGetValue(property, StringComparison.InvariantCultureIgnoreCase, out var value))
        {
            try
            { 
                return value.ToObject<T>();
            }
            catch (Exception e)
            {
                if (logger is not null)
                    LogMessages.JsonConversionFailed(logger, property, typeof(T).Name, e);
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// Returns the array with the specified name from the JSON object. Inserts a new empty array if the property doesn't exist.
    /// </summary>
    public static JArray EnsureArray(this JObject parent, string name)
    {
        if (parent == null) throw new ArgumentNullException(nameof(parent));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

        if (parent[name] is not JArray array)
        {
            parent.Add(name, new JArray());
            array = (JArray)parent[name]!;
        }
        return array;
    }

    /// <summary>
    /// Returns the object with the specified name from the JSON object. Inserts a new empty object if the property doesn't exist.
    /// </summary>
    public static JObject EnsureObject(this JObject parent, string name)
    {
        if (parent == null) throw new ArgumentNullException(nameof(parent));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

        if (parent[name] is not JObject obj)
        {
            parent.Add(name, new JObject());
            obj = (JObject)parent[name]!;
        }
        return obj;
    }

    /// <summary>
    /// Adds the specified converters to the <see cref="JsonSerializerSettings"/> instance.
    /// </summary>
    public static JsonSerializerSettings WithConverters(this JsonSerializerSettings settings, params JsonConverter[]? converters)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (converters is { Length: > 0 })
            Array.ForEach(converters, settings.Converters.Add);
        return settings;
    }

    /// <summary>
    /// Safely deserializes the JSON to the type <typeparamref name="T"/>, returning the result in an out parameter.
    /// </summary>
    public static bool TryParseJson<T>(this string json, out T? result) where T : class
    {
        try
        {
            result = JsonConvert.DeserializeObject<T>(json);
            return result != default;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Safely parses a JSON object from a string, returning the result in an out parameter.
    /// </summary>
    public static bool TryParseJsonObject(this string json, out JObject? result)
    {
        try
        {
            result = JObject.Parse(json);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }        

    /// <summary>
    /// Safely parses a JSON array from a string, returning the result in an out parameter.
    /// </summary>
    public static bool TryParseJsonArray(this string json, out JArray? result)
    {
        try 
        {
            result = JArray.Parse(json);
            return true;
        }
        catch 
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Serializes the object to a JSON string, with specified formatting.
    /// </summary>
    public static string ToJsonString<T>(this T? value, Newtonsoft.Json.Formatting format = Newtonsoft.Json.Formatting.Indented) where T : class =>
        value == default
            ? ""
            : JsonConvert.SerializeObject(value, new JsonSerializerSettings { Formatting = format, NullValueHandling = NullValueHandling.Ignore });

}
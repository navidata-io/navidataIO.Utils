// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

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
    public static T? ReadPropertySafe<T>(this JObject json, string property, T? defaultValue = default, ILogger? logger = null)
    { 
        if (json == null) throw new ArgumentNullException(nameof(json));
        if (string.IsNullOrEmpty(property)) throw new ArgumentException("Value cannot be null or empty.", nameof(property));

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
            return result != null;
        }
        catch
        {
            result = null;
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
            result = null;
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
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Serializes the object to a JSON string, with specified formatting.
    /// </summary>
    [Obsolete("Use AsJson<T>(T?, Formatting) instead.")]
    public static string ToJsonString<T>(this T? value, Newtonsoft.Json.Formatting format = Newtonsoft.Json.Formatting.Indented) where T : class =>
        value == null
            ? ""
            : JsonConvert.SerializeObject(value, new JsonSerializerSettings { Formatting = format, NullValueHandling = NullValueHandling.Ignore });

    /// <summary>
    /// Creates or overwrites the named property on a <see cref="JObject"/>, but only if the provided value is not <c>null</c>.
    /// </summary>
    public static JObject WithOptional(this JObject json, string propertyName, JToken? value)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        if (value is null || value is { Type: JTokenType.Null }) return json;
        if (value is JValue { Value: null } /* default(string) */) return json;

        json[propertyName] = value;
        return json;
    }

    /// <summary>
    /// Creates or overwrites the named property on a <see cref="JObject"/>, but only if the provided value is not <c>null</c>
    /// and the provided factory method produces a non-null <see cref="JToken"/>.
    /// </summary>
    public static JObject WithOptional<T>(this JObject json, string propertyName, T? value, Func<T, JToken?> createToken)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
        
        if (value is null) return json;

        if (createToken(value) is { } token)
            json[propertyName] = token;
        return json;
    }

    /// <summary>
    /// Creates or overwrites the named JSON property as a new <see cref="JArray"/>,
    /// generated from items of the provides collection using the provided <paramref name="createToken"/> factory method,
    /// but only if the provided collection is not <c>null</c>. Any items producing a <c>null</c> token are ignored.
    /// </summary>
    public static JObject WithOptionalArray<T>(this JObject parent, string propertyName,
        ICollection<T>? collection, Func<T, JToken?> createToken)
    {
        if (parent == null) throw new ArgumentNullException(nameof(parent));
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        if (collection is null) return parent;

        parent[propertyName] = new JArray(collection.Select(createToken).WhereNotNull());
        return parent;
    }

    /// <summary>
    /// Creates or overwrites the named property on a <see cref="JObject"/>, regardless of the provided value.
    /// </summary>
    public static JObject WithProperty(this JObject json, string propertyName, JToken? value)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
        json[propertyName] = value;
        return json;
    }

    /// <summary>
    /// Selects a <see cref="JObject"/> from the provided JSON at the specified path.
    /// Throws an <see cref="InvalidOperationException"/> if the token is not a <see cref="JObject"/> or does not exist.
    /// </summary>
    public static JObject RequireObject(this JObject json, string jsonPath)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        return json.SelectObject(jsonPath, throwIfNull: true)!;
    }

    /// <summary>
    /// Selects a <see cref="JObject"/> from the provided JSON at the specified path.
    /// Returns <c>null</c> if the token does not exist, unless <paramref name="throwIfNull"/> is <c>true</c>,
    /// in which case an <see cref="InvalidOperationException"/> is thrown.
    /// Always throws an <see cref="InvalidOperationException"/> if the token is not a <see cref="JObject"/>.
    /// </summary>
    public static JObject? SelectObject(this JObject json, string jsonPath, bool throwIfNull = false) =>
        (json ?? throw new ArgumentNullException(nameof(json))).SelectToken(jsonPath) switch
        {
            JObject jObject => jObject,
            null when throwIfNull => throw new InvalidOperationException($"Expected token at path {jsonPath} does not exist and 'throwIfNull' was selected."),
            null => null,
            _ => throw new InvalidOperationException($"Unable to select object from JSON at path: {jsonPath}")
        };

    /// <summary>
    /// Renames a property on a <see cref="JObject"/>. If the property does not exist, it is ignored.
    /// </summary>
    public static JObject RenameProperty(this JObject parent, string oldName, string newName)
    {
        if (parent[oldName] is { } existingToken)
        {
            parent[newName] = existingToken;
            parent.Remove(oldName);
        }
        return parent;
    }

    /// <summary>
    /// Removes the specified properties from a <see cref="JObject"/>. If a property does not exist, it is ignored.
    /// </summary>
    public static JObject RemoveProperties(this JObject obj, params string[] propertyNames)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        foreach (var propertyName in propertyNames)
        {
            obj.Remove(propertyName);
        }
        return obj;
    }

    /// <summary>
    /// Serializes the object to a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="formatting">The (optional) <see cref="Formatting"/> to apply when serializing. If omitted, and no <paramref name="serializerSettings"/> are provided, <see cref="Formatting.Indented"/> is used. If set, and <paramref name="serializerSettings"/> are provided, this argument overwrites <see cref="JsonSerializerSettings.Formatting"/>.</param>
    /// <param name="serializerSettings">The (optional) <see cref="JsonSerializerSettings"/> to use when serializing.</param>
    /// <param name="resultIfNull">The value to return if <paramref name="obj"/> is null (default is <c>null</c>).</param>
    /// <returns>The serialized object as a string.</returns>
    public static string? AsJson<T>(this T? obj, Formatting? formatting = null,
        JsonSerializerSettings? serializerSettings = null, string? resultIfNull = null)
    {
        if (obj is null) return resultIfNull;

        var settings = serializerSettings ?? new JsonSerializerSettings();
        if (serializerSettings is null && formatting is null)
            settings.Formatting = Formatting.Indented;
        else if (formatting is not null)
            settings.Formatting = formatting.Value;

        return obj.AsJson(JsonSerializer.Create(settings));
    }

    /// <summary>
    /// Serializes the object to a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="serializer">The <see cref="JsonSerializer"/> to use when serializing.</param>
    /// <param name="resultIfNull">The value to return if <paramref name="obj"/> is null (default is <c>null</c>).</param>
    /// <returns>The serialized object as a string.</returns>
    public static string? AsJson<T>(this T? obj, JsonSerializer serializer, string? resultIfNull = null)
    {
        if (obj is null) return resultIfNull;
        if (serializer == null) throw new ArgumentNullException(nameof(serializer));

        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            serializer.Serialize(writer, obj);
        }
        return sb.ToString();
    }


    /// <summary>
    /// Creates a <see cref="JToken"/> from the provided object using the specified <see cref="JsonSerializer"/>.
    /// Returns <c>null</c> if the object is <c>null</c>.
    /// </summary>
    /// <param name="obj">The object to convert to JSON.</param>
    /// <param name="formatting">The (optional) <see cref="Formatting"/> to apply when serializing. If omitted, and no <paramref name="serializerSettings"/> are provided, <see cref="Formatting.Indented"/> is used. If set, and <paramref name="serializerSettings"/> are provided, this argument overwrites <see cref="JsonSerializerSettings.Formatting"/>.</param>
    /// <param name="serializerSettings">The (optional) <see cref="JsonSerializerSettings"/> to use when serializing.</param>
    public static JToken? ToJson<T>(this T obj, Formatting? formatting = null,
        JsonSerializerSettings? serializerSettings = null)
    {
        var settings = serializerSettings ?? new JsonSerializerSettings();
        if (serializerSettings is null && formatting is null)
            settings.Formatting = Formatting.Indented;
        else if (formatting is not null)
            settings.Formatting = formatting.Value;
        var serializer = JsonSerializer.Create(settings);

        return obj.ToJson(serializer);
    }

    /// <summary>
    /// Creates a <see cref="JToken"/> from the provided object using the specified <see cref="JsonSerializer"/>.
    /// Returns <c>null</c> if the object is <c>null</c>.
    /// </summary>
    public static JToken? ToJson<T>(this T obj, JsonSerializer serializer)
    {
        object? o = obj;
        return o is null
            ? null
            : JToken.FromObject(o, serializer ?? throw new ArgumentNullException(nameof(serializer)));
    }

}
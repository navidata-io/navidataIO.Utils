using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace navidataIO.Utils.Json;

/// <summary>
/// Base class for a Json serializable type with support for unmapped properties.
/// </summary>
public abstract class JObjectBase
{
    [JsonExtensionData]
    protected IDictionary<string, JToken?>? AdditionalProperties;
}

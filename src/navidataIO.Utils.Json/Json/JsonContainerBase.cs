// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace navidataIO.Utils.Json;

/// <summary>
/// Base class for a Json serializable type with support for unmapped properties.
/// </summary>
public abstract class JObjectBase
{
    /// <summary>
    /// Gets or sets the additional Json properties that are not explicitly defined in the class.
    /// </summary>
    [JsonExtensionData]
    protected IDictionary<string, JToken?>? AdditionalProperties;
}

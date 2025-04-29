// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

namespace navidataIO.Utils;

internal static partial class LogMessages
{

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Error,
        Message = "Json conversion error occurred reading Property {PropertyName} as Type {TypeName}")]
    internal static partial void JsonConversionFailed(ILogger logger, string propertyName, string typeName, Exception e);

}
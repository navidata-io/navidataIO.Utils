using Microsoft.Extensions.Logging;

namespace FabricTools.Utils;

internal static partial class LogMessages
{

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Error,
        Message = "Json conversion error occurred reading Property {PropertyName} as Type {TypeName}")]
    internal static partial void JsonConversionFailed(ILogger logger, string propertyName, string typeName, Exception e);

}
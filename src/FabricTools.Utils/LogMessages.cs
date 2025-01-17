namespace FabricTools.Utils;

internal static partial class LogMessages
{

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Debug,
        Message = "Created TEMP folder at `{Path}`")]
    internal static partial void CreatedTempFolder(ILogger logger, string path);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Debug,
        Message = "Deleted TEMP folder at `{Path}`")]
    internal static partial void DeletedTempFolder(ILogger logger, string path);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "Failed to delete TEMP folder at `{Path}`")]
    internal static partial void DeleteTempFolderFailed(ILogger logger, string path, Exception exception);

}
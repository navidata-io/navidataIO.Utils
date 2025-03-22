namespace navidataIO.Utils;

/// <summary>
/// Represents a dynamically generated temporary folder with an explicitly controlled lifetime.
/// </summary>
public sealed class TempFolder : LoggingBase, IDisposable
{
    /// <summary>
    /// Gets the full path of the temporary folder.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Determined whether the temporary folder should be deleted when disposed. Default is <c>true</c>.
    /// </summary>
    public bool Delete { get; set; } = true;

    /// <summary>
    /// Creates a temporary folder in the system's temporary folder.
    /// </summary>
    public TempFolder(ILoggerFactory? loggerFactory = default) : base(loggerFactory)
    {
        var path = System.IO.Path.GetTempFileName();
        File.Delete(path);

        Path = EnsureFolder(path);
    }

    /// <summary>
    /// Creates a temporary folder at the specified path.
    /// </summary>
    public TempFolder(string path, ILoggerFactory? loggerFactory = default) : base(loggerFactory)
    {
        Path = EnsureFolder(path);
    }

    private string EnsureFolder(string path)
    {
        var tempPath = System.IO.Path.GetFullPath(path);
        Directory.CreateDirectory(tempPath);

        LogMessages.CreatedTempFolder(Logger, tempPath);
        return tempPath;
    }

    /// <summary>
    /// Combines the path segments specified with the base path of this instance and returns the resulting path.
    /// </summary>
    public string GetPath(params string[] paths)
        => System.IO.Path.Combine(new[] { this.Path }.Concat(paths).ToArray());

    void IDisposable.Dispose()
    {
        if (!Delete) return;

        try
        {
            Directory.Delete(Path, recursive: true);
            LogMessages.DeletedTempFolder(Logger, Path);
        }
        catch (IOException ex)
        {
            LogMessages.DeleteTempFolderFailed(Logger, Path, ex);
        }
    }
}

// Copyright (c) 2024 pbi-tools Ltd, London

using System.Text;

namespace navidataIO.Utils.IO;

/// <summary>
/// Contains various extension methods for file and directory paths.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    /// Returns the file name of the specified path string without the extension.
    /// </summary>
    public static string WithoutExtension(this string path) =>
        Path.GetFileNameWithoutExtension(path ?? throw new ArgumentNullException(nameof(path)));

    /// <summary>
    /// Returns the file extension converted to lowercase.
    /// </summary>
    public static string GetExtension(this string path) =>
        Path.GetExtension(path ?? throw new ArgumentNullException(nameof(path)))
            .ToLowerInvariant();


    /// <summary>
    /// Removes the last segment from the path.
    /// </summary>
    public static RelativeFilePath RemoveLast(this RelativeFilePath path) => path.Segments.Length switch
    {
        0 => path,
        _ => new RelativeFilePath(path.Segments.Take(path.Segments.Length - 1), path.DirectorySeparator)
    };

    /// <summary>
    /// Ensures the specified path ends with a directory separator.
    /// </summary>
    public static string EnsureEndsInDirectorySeparator(this string path, char? customDirectorySeparator = default) => path switch
    {
        null => throw new ArgumentNullException(nameof(path)),
        _ when path.EndsWith("/") || path.EndsWith("\\") => path,
        _ => path + (customDirectorySeparator ?? Path.DirectorySeparatorChar)
    };

    private static readonly Dictionary<char, string> FilenameCharReplace = "\"<>|:*?/\\".ToCharArray()
        .ToDictionary(c => c, c => $"%{((int)c):X}");
    // Note - This can be reversed via WebUtility.UrlDecode()

    /// <summary>
    /// Sanitizes the specified string to be used as a filename.
    /// Any invalid characters are replaced with their URL-encoded equivalent.
    /// </summary>
    public static string? SanitizeFilename(this string? name)
    {
        if (name == null) return null;

        var sb = new StringBuilder();
        foreach (var c in name)
        {
            if (FilenameCharReplace.TryGetValue(c, out var s))
                sb.Append(s);
            else
                sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Unsanitizes the specified filename by url decoding the string.
    /// </summary>
    public static string UnsanitizeFilename(this string name) => System.Net.WebUtility.UrlDecode(name);
}
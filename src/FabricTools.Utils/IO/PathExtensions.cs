// Copyright (c) 2024 pbi-tools Ltd, London

namespace FabricTools.Utils.IO;

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

}
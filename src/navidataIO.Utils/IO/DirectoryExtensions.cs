// ReSharper disable once CheckNamespace
namespace System.IO;

/// <summary>
/// Provides extension methods for <see cref="DirectoryInfo"/>.
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// Deletes the directory if it exists, then creates it.
    /// </summary>
    public static void Recreate(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo is null) throw new ArgumentNullException(nameof(directoryInfo));
        if (directoryInfo.Exists) directoryInfo.Delete(recursive: true);
        directoryInfo.Create();
    }

    /// <summary>
    /// Deletes all files and subdirectories in the specified directory.
    /// </summary>
    public static void Clean(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo is null) throw new ArgumentNullException(nameof(directoryInfo));
        if (!directoryInfo.Exists) return;

        // Delete all files
        foreach (var file in directoryInfo.GetFiles())
        {
            file.Delete();
        }

        // Delete all subdirectories
        foreach (var subDirectory in directoryInfo.GetDirectories())
        {
            subDirectory.Delete(true);
        }
    }

    /// <summary>
    /// Ensures the directory exists and deletes any files or sub-folders inside the directory.
    /// </summary>
    public static void EnsureEmpty(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo is null) throw new ArgumentNullException(nameof(directoryInfo));

        if (directoryInfo.Exists)
        {
            directoryInfo.Clean();
        }
        else
        {
            directoryInfo.Create();
        }
    }
}
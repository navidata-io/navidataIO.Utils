// ReSharper disable once CheckNamespace
namespace System.IO;

public static class DirectoryExtensions
{
    /// <summary>
    /// Deletes the directory if it exists, then creates it.
    /// </summary>
    public static void Recreate(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists) directoryInfo.Delete(recursive: true);
        directoryInfo.Create();
    }

    /// <summary>
    /// Deletes all files and subdirectories in the specified directory.
    /// </summary>
    public static void Clean(this DirectoryInfo directoryInfo)
    {
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
    /// Ensures the directory exists and cleans it if it contains and files or subfolders.
    /// </summary>
    public static void EnsureEmpty(this DirectoryInfo directoryInfo)
    {
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
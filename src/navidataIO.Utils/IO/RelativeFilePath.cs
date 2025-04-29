// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

namespace navidataIO.Utils.IO;

/// <summary>
/// Represents an immutable relative file path and provides convenience methods for accessing path segments and combining paths.
/// </summary>
public readonly struct RelativeFilePath : IEquatable<RelativeFilePath>
{
    /// <summary>
    /// An empty <see cref="RelativeFilePath"/>.
    /// </summary>
    public static RelativeFilePath Empty { get; } = new([]);

    /// <summary>
    /// An array of the path segments.
    /// </summary>
    public string[] Segments { get; } = [];

    /// <summary>
    /// The directory separator character used when creating a joined path via <see cref="ToString"/>.
    /// </summary>
    public char DirectorySeparator { get; } = Path.DirectorySeparatorChar;

    /// <summary>
    /// Creates a new instance of <see cref="RelativeFilePath"/>.
    /// </summary>
    /// <param name="pathSegments">An array of path segments.</param>
    /// <param name="directorySeparator">The optional directory separator char for this instance. <see cref="Path.DirectorySeparatorChar"/> is used if this argument is not provided.</param>
    public RelativeFilePath(IEnumerable<string> pathSegments, char? directorySeparator = null)
    {
        if (pathSegments == null)
            throw new ArgumentNullException(nameof(pathSegments));

        this.Segments = pathSegments.ToArray();
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var segment in Segments)
        {
            if (string.IsNullOrWhiteSpace(segment)) throw new ArgumentException("Path segments must not be empty.", nameof(pathSegments));
            if (invalidChars.Any(segment.Contains)) throw new ArgumentException($"Path segment contains invalid characters: '{segment}'", nameof(pathSegments));
        }

        this.DirectorySeparator = directorySeparator ?? Path.DirectorySeparatorChar;
    }

    /// <summary>
    /// Clones the original <see cref="RelativeFilePath"/> instance,
    /// optionally with a specified directory separator char.
    /// </summary>
    public RelativeFilePath(RelativeFilePath original, char? directorySeparator = null) : this(original.Segments, directorySeparator ?? original.DirectorySeparator)
    {
    }

    /// <summary>
    /// Joins the segments into a single path string.
    /// </summary>
    public override string ToString() => string.Join(DirectorySeparator.ToString(), Segments);

    /// <summary>
    /// Return the file name of the path.
    /// </summary>
    public string FileName => Segments.Length > 0 ? Segments[Segments.Length - 1] : string.Empty;

    /// <summary>
    /// Combines two <see cref="RelativeFilePath"/> instances.
    /// </summary>
    public static RelativeFilePath operator +(RelativeFilePath left, RelativeFilePath right) => new(left.Segments.Concat(right.Segments).ToArray());

    /// <summary>
    /// Appends a <see cref="RelativeFilePath"/> instance to a base string path.
    /// </summary>
    public static string operator +(string left, RelativeFilePath right) => Path.Combine(left, right);

    private static bool SkipSegment(string s) => s == "/" || s.Contains(':') || s == string.Empty;

    /// <summary>
    /// Converts the path string to a <see cref="RelativeFilePath"/> instance.
    /// </summary>
    public static implicit operator RelativeFilePath(string path) => Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri) switch
    {
        true when uri.IsAbsoluteUri => new(uri.Segments
            .SkipWhile(SkipSegment)
            .Select(s => s.TrimEnd('/'))),
        _ => new(path.Split('/', '\\')
            .SkipWhile(SkipSegment))
    };

    /// <inheritdoc cref="ToString"/>
    public static implicit operator string(RelativeFilePath path) => path.ToString();

    /// <summary>
    /// Creates a new instance of <see cref="RelativeFilePath"/> from the full path provided,
    /// relative to the given base path.
    /// </summary>
    public static RelativeFilePath Create(Uri fullPath, Uri basePath, char? directorySeparator = null)
        => new(basePath.MakeRelativeUri(fullPath).OriginalString.Split('/', '\\'), directorySeparator);

    /// <summary>
    /// Creates a new instance of <see cref="RelativeFilePath"/> from the full path provided,
    /// relative to the given base path.
    /// Note that the base path must end with a directory separator for the last segment to be considered part of the base path.
    /// </summary>
    public static RelativeFilePath Create(string fullPath, string basePath, char? directorySeparator = null)
        => new(new Uri(basePath).MakeRelativeUri(new Uri(fullPath)).OriginalString.Split('/', '\\').Select(System.Net.WebUtility.UrlDecode), directorySeparator);

    #region IEquatable<RelativeFilePath> implementation

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is RelativeFilePath other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (Segments == null) return 0;

        unchecked
        {
            return Segments.Aggregate(17, (hash, item) => hash * 31 + (item?.GetHashCode() ?? 0));
        }
    }

    /// <inheritdoc />
    public bool Equals(RelativeFilePath other) =>
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        (this.Segments == null && other.Segments == null)
        || (this.Segments?.Length == other.Segments?.Length
            && this.Segments.SequenceEqual(other.Segments, StringComparer.InvariantCulture /* ensures the comparison is culture-invariant */));

    #endregion
}

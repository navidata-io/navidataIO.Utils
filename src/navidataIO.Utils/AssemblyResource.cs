// Copyright (c) 2024 pbi-tools Ltd, London

using System.Collections;
using System.Reflection;
using System.Text;

namespace navidataIO.Utils;

/// <summary>
/// An embedded resource in an <see cref="Assembly"/>.
/// </summary>
public class AssemblyResource
{
    private readonly Assembly _assembly;
    private readonly Func<string, string> _pathConverter;

    /// <summary>
    /// Gets the full resource name.
    /// </summary>
    public string Name { get; }

    internal AssemblyResource(Assembly assembly, string name, Func<string, string> pathConverter)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        this._assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        this.Name = name;
        _pathConverter = pathConverter;
    }


    /// <summary>
    /// Returns a <see cref="Stream"/> for the embedded resource.
    /// </summary>
    public Stream GetStream() => _assembly.GetManifestResourceStream(Name) ?? throw new IOException();

    /// <summary>
    /// Transforms the embedded resource into a value of <typeparamref name="T"/> using the specified function.
    /// </summary>
    public T Get<T>(Func<Stream, T> transform)
    {
        using var stream = GetStream();
        return transform(stream);
    }

    /// <summary>
    /// Transforms the embedded resource into a string using the specified encoding.
    /// <see cref="Encoding.UTF8"/> is used by default.
    /// </summary>
    public string GetString(Encoding? encoding = null)
    {
        using var stream = GetStream();
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Asynchronously extracts the embedded resource to the specified folder and returns to full path.
    /// The full file path is determined by combining the <paramref name="folderPath"/> with the resource name,
    /// after applying the <see cref="AssemblyResources.PathConverter"/> function to the name.
    /// </summary>
    public async Task<string> ExtractToAsync(string folderPath, bool overwrite = true)
    {
        var targetFile = new FileInfo(Path.Combine(folderPath, _pathConverter(Name)));
        if (targetFile.Exists && !overwrite)
            throw new IOException($"The file at {targetFile.FullName} exists and the overwrite flag was not set.");

        targetFile.Directory!.Create();

        using var stream = GetStream();
        using var fileStream = targetFile.OpenWrite();
        await stream.CopyToAsync(fileStream);

        return targetFile.FullName;
    }

}

/// <summary>
/// Represents the collection of embedded resources in a given <see cref="Assembly"/>.
/// </summary>
public class AssemblyResources : IEnumerable<AssemblyResource>
{
    private readonly Assembly _assembly;
    private readonly string[] _names;

    /// <summary>
    /// Creates a new instance of <see cref="AssemblyResources"/> for the specified <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to scan for resources. If not provided, the calling assembly is used.</param>
    public AssemblyResources(Assembly? assembly = null)
    {
        _assembly = assembly ?? Assembly.GetCallingAssembly();
        _names = _assembly.GetManifestResourceNames();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to perform strict matching.
    /// </summary>
    public bool StrictMatching { get; set; } = false;

    /// <summary>
    /// Gets or sets a function that converts the resource name into a (relative) file path.
    /// Only needed when extracting resources to the file system via <see cref="AssemblyResource.ExtractToAsync"/>.
    /// </summary>
    public Func<string, string> PathConverter { get; set; } = p => p;

    /// <summary>
    /// Gets an embedded resource with a matching name.
    /// If no exact match is found, and <see cref="StrictMatching"/> is disabled,
    /// the first resource with a name ending in the specified term is returned,
    /// otherwise the first resource with a name starting with the specified term.
    /// </summary>
    /// <exception cref="ArgumentException">No matching resource could be found.</exception>
    public AssemblyResource this[string name] => _names switch
    { 
        _ when _names.Contains(name)
            => new AssemblyResource(_assembly, name, PathConverter),
        _ when !StrictMatching && _names.FirstOrDefault(n => n.EndsWith(name)) is {} match
            => new AssemblyResource(_assembly, match, PathConverter),
        _ when !StrictMatching && _names.FirstOrDefault(n => n.StartsWith(name)) is {} match
            => new AssemblyResource(_assembly, match, PathConverter),
        _
            => throw new ArgumentException($"Embedded resource '{name}' not found.", nameof(name))
    };

    /// <summary>
    /// Returns all embedded resources with the specified name prefix.
    /// </summary>
    public IEnumerable<AssemblyResource> StartsWith(string prefix) => _names
        .Where(n => n.StartsWith(prefix))
        .Select(n => new AssemblyResource(_assembly, n, PathConverter));

    #region IEnumerable

    /// <inheritdocs/>
    public IEnumerator<AssemblyResource> GetEnumerator() => _names.Select(n => new AssemblyResource(_assembly, n, PathConverter)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}
using System.Reflection;

namespace navidataIO.Utils;

/// <summary>
/// Provides information about the version of an assembly.
/// </summary>
public class AssemblyVersionInformation
{
    /// <summary>
    /// The consolidated version of the assembly. Returns the informational version if present, otherwise the file version, otherwise the assembly version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// The assembly version, if present.
    /// </summary>
    public string? AssemblyVersion { get; }
    
    /// <summary>
    /// The assembly file version, if present.
    /// </summary>
    public string? FileVersion { get; }
    
    /// <summary>
    /// The assembly informational version, if present.
    /// </summary>
    public string? InformationalVersion { get; }

    private AssemblyVersionInformation(Assembly assembly)
    {
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        AssemblyVersion = assembly.GetName().Version?.ToString();
        FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        Version = InformationalVersion ?? FileVersion ?? AssemblyVersion ?? "Unknown";
    }

    /// <summary>
    /// Creates an instance of <see cref="AssemblyVersionInformation"/> from the specified <paramref name="assembly"/>.
    /// </summary>
    public static AssemblyVersionInformation FromAssembly(Assembly assembly) => new AssemblyVersionInformation(assembly ?? throw new ArgumentNullException(nameof(assembly)));

    /// <summary>
    /// Creates an instance of <see cref="AssemblyVersionInformation"/> from the assembly of the calling method.
    /// </summary>
    public static AssemblyVersionInformation FromCallingAssembly() => new AssemblyVersionInformation(Assembly.GetCallingAssembly());

    /// <summary>
    /// Creates an instance of <see cref="AssemblyVersionInformation"/> from the assembly of the specified <typeparamref name="T"/>.
    /// </summary>
    public static AssemblyVersionInformation FromType<T>() => new AssemblyVersionInformation(typeof(T).Assembly);
}
// Copyright (c) 2024 pbi-tools Ltd, London

using System.Reflection;

namespace navidataIO.Utils;

public class AssemblyVersionInformation
{
    public string Version { get; }
    public string? AssemblyVersion { get; }
    public string? FileVersion { get; }
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

    public static AssemblyVersionInformation FromAssembly(Assembly assembly) => new AssemblyVersionInformation(assembly);

    public static AssemblyVersionInformation FromCallingAssembly() => new AssemblyVersionInformation(Assembly.GetCallingAssembly());

    public static AssemblyVersionInformation FromType<T>() => new AssemblyVersionInformation(typeof(T).Assembly);
}
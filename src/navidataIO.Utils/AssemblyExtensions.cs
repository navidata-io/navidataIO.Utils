// Copyright (c) 2024 pbi-tools Ltd, London

using System.Reflection;

namespace navidataIO.Utils;

/// <summary>
/// 
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Returns the AssemblyInformationalVersion of the assembly containing the object, or <c>null</c>
    /// if the attribute does not exist.
    /// </summary>
    public static string? GetAssemblyInformationalVersion<T>(this T _)
    {
        var attribute = typeof(T).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        return attribute?.InformationalVersion;
    }
}
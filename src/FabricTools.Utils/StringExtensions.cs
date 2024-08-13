// Copyright (c) 2024 pbi-tools Ltd, London

using System.Text;

namespace FabricTools.Utils;

/// <summary>
/// Contains various string extension methods.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Capitalizes the provided string.
    /// </summary>
    public static string? ToPascalCase(this string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        var sb = new StringBuilder(s)
        {
            [0] = char.ToUpper(s![0])
        };
        return sb.ToString();
    }

    /// <summary>
    /// Converts the first character of the provided string to lowercase.
    /// </summary>
    public static string? ToCamelCase(this string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        var sb = new StringBuilder(s)
        {
            [0] = char.ToLower(s![0])
        };
        return sb.ToString();
    }

    /// <summary>
    /// Returns <c>true</c> if the value is not null or whitespace, otherwise <c>false</c>.
    /// </summary>
    public static bool HasValue(this string value) => !string.IsNullOrWhiteSpace(value);

}
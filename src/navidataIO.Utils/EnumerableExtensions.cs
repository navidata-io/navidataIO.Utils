// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

namespace navidataIO.Utils;

/// <summary>
/// Provides LINQ extension methods.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Filters a collection of nullable values, passing through only the non-null values.
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) => source.Where(x => x is not null)!;
}
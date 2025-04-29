// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

// ReSharper disable once CheckNamespace
namespace Xunit;

/// <summary>
/// Skips the tests and marks it as Not Implemented.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TestNotImplementedAttribute : FactAttribute
{
    /// <inheritdocs/>
    public TestNotImplementedAttribute()
    {
        base.Skip = "Test Not Implemented";
    }
}

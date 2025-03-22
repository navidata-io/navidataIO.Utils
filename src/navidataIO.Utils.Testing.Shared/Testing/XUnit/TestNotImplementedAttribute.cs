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

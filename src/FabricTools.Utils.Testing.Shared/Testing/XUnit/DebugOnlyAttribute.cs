using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace Xunit;

/// <summary>
/// A Xunit test that is skipped unless a debugger is attached.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DebugOnlyAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DebugOnlyAttribute"/> class.
    /// </summary>
    public DebugOnlyAttribute()
    {
        if (!Debugger.IsAttached)
        {
            Skip = "Only running in interactive mode.";
        }
    }
}
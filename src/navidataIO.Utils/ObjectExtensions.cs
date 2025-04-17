namespace navidataIO.Utils;

/// <summary>
/// Contains various extension methods for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Invokes the specified action with the target object as an argument
    /// and return the object, allowing for fluent chaining.
    /// </summary>
    /// <exception cref="ArgumentNullException">The target object is a null reference.</exception>
    public static T With<T>(this T target, Action<T>? action)
    {
        action?.Invoke(target ?? throw new ArgumentNullException(nameof(target)));
        return target;
    }
}
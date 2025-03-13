namespace navidataIO.Utils;

public static class ObjectExtensions
{
    public static T With<T>(this T target, Action<T>? action)
    {
        action?.Invoke(target ?? throw new ArgumentNullException(nameof(target)));
        return target;
    }
}
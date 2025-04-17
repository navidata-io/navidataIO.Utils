using System.Text;

namespace navidataIO.Utils;

/// <summary>
/// Contains various string extension methods.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Capitalizes all words and removes whitespace from the string.
    /// </summary>
    public static string? ToPascalCase(this string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        return (s!.Split([' '], StringSplitOptions.RemoveEmptyEntries) switch
            {
                { Length: 1 } words => MakeUpperCase(words[0]),
                var words => words.Aggregate(
                    new StringBuilder(),
                    (sb, word) => sb.Append(MakeUpperCase(word)))
            })
            .ToString();
    }

    private static StringBuilder MakeLowerCase(string s) => new(s)
    {
        [0] = char.ToLower(s![0])
    };

    private static StringBuilder MakeUpperCase(string s) => new(s)
    {
        [0] = char.ToUpper(s![0])
    };

    /// <summary>
    /// Converts the first character of the provided string to lower-case, removes whitespace and capitalizes
    /// all subsequent words.
    /// </summary>
    public static string? ToCamelCase(this string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        return (s!.Split([' '], StringSplitOptions.RemoveEmptyEntries) switch
            {
                { Length: 1 } words => MakeLowerCase(words[0]),
                var words => words.Skip(1).Aggregate(
                    MakeLowerCase(words[0]),
                    (sb,word) => sb.Append(MakeUpperCase(word)))
            })
            .ToString();
    }

    /// <summary>
    /// Returns <c>true</c> if the value is not null or whitespace, otherwise <c>false</c>.
    /// </summary>
    public static bool HasValue(this string value) => !string.IsNullOrWhiteSpace(value);

}
namespace Asreyion.Modules.SimpleContent.Utilities;

public static class SlugFormatter
{
    private static readonly HashSet<string> UpperWords =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "xiv", "xiii", "xii", "xi", "x",
            "api", "ui", "ux", "id", "url"
        };

    public static string ToTitle(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return string.Empty;
        }

        return string.Join(" ",
            slug.Split('-', StringSplitOptions.RemoveEmptyEntries)
                .Select(FormatWord)
        );
    }

    public static string ToCanonicalSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return string.Empty;
        }

        return string.Join("-",
            slug.Split('-', StringSplitOptions.RemoveEmptyEntries)
                .Select(FormatWord)
        );
    }

    private static string FormatWord(string word)
    {
        if (UpperWords.Contains(word))
        {
            return word.ToUpperInvariant();
        }

        return char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();
    }
}
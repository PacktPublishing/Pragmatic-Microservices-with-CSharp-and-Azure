namespace Codebreaker.Data.SqlServer;

public static class MappingExtensions
{
    public static ICollection<T> ToFieldCollection<T>(this string fields)
        where T : IParsable<T>
    {
        return fields.Split('#')
            .Select(field => T.Parse(field, default))
            .ToList();
    }

    public static string ToFieldString<T>(this IEnumerable<T> fields)
        where T : notnull
    {
        return string.Join('#', fields.Select(field => field.ToString()));
    }

    public static T[] ToFieldArray<T>(this string fields)
    where T : IParsable<T>
    {
        return fields.Split('#')
            .Select(field => T.Parse(field, default))
            .ToArray();
    }

    public static string ToFieldString<T>(this T[] fields)
    where T : notnull
    {
        return string.Join('#', fields.Select(field => field.ToString()));
    }

    public static string ToLookupString(this ILookup<string, string> lookup)
    {
        return string.Join('#', lookup.SelectMany(group => group.Select(item => $"{group.Key}:{item}")));
    }

    public static ILookup<string, string> ToLookup(this string lookup)
    {
        string[] data = lookup.Split("#");

        return data.ToLookup(keySelector: s => s.Split(":")[0], elementSelector: s => s.Split(":")[1]);
    }
}
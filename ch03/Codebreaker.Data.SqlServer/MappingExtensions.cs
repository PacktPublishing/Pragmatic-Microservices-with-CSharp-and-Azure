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

    public static string ToFieldString<T>(this T[] fields)
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

    public static string ToFieldsString(this IDictionary<string, IEnumerable<string>> fields)
    {
        return string.Join(
            '#', fields.SelectMany(
                field => field.Value
                    .Select(value => $"{field.Key}:{value}")));
    }

    public static IDictionary<string, IEnumerable<string>> ToFieldsDictionary(this string fieldsString)
    {
        Dictionary<string, List<string>> fields = new();

        foreach (var pair in fieldsString.Split('#'))
        {
            var index = pair.IndexOf(':');

            if (index < 0)
            {
                throw new ArgumentException($"Field {pair} does not contain ':' delimiter.");
            }

            var key = pair[..index];
            var value = pair[(index + 1)..];

            if (!fields.TryGetValue(key, out var list))
            {
                list = new List<string>();
                fields[key] = list;
            }

            list.Add(value);
        }

        return fields.ToDictionary(
            pair => pair.Key,
            pair => (IEnumerable<string>)pair.Value);
    }
}
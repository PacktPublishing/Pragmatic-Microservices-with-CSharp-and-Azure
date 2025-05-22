namespace Codebreaker.Data.Postgres;

public static class MappingExtensions
{
    public static ICollection<T> ToFieldCollection<T>(this string fields)
        where T : IParsable<T>
    {
        return [.. fields.Split('#').Select(field => T.Parse(field, default))];
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
        return [.. fields.Split('#').Select(field => T.Parse(field, default))];
    }

    public static string ToFieldsString(this IDictionary<string, IEnumerable<string>> fields)
    {
        return string.Join(
            '#', fields.SelectMany(
                key => key.Value
                    .Select(value => $"{key.Key}:{value}")));
    }

    public static IDictionary<string, IEnumerable<string>> FromFieldsString(this string fieldsString)
    {
        Dictionary<string, List<string>> fields = [];

        foreach (string pair in fieldsString.Split('#'))
        {
            int index = pair.IndexOf(':');

            if (index < 0)
            {
                throw new ArgumentException($"Field {pair} does not contain ':' delimiter.");
            }

            string key = pair[..index];
            string value = pair[(index + 1)..];

            if (!fields.TryGetValue(key, out List<string>? list))
            {
                list = [];
                fields[key] = list;
            }

            list.Add(value);
        }

        return fields.ToDictionary(
            pair => pair.Key,
            pair => (IEnumerable<string>)pair.Value);
    }
}
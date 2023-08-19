namespace Codebreaker.GameAPIs.Extensions;

public static class FieldExtensions
{
    public static IEnumerable<T> ToFields<T>(this string[] fieldStrings)
        where T : IParsable<T>
    {
        foreach (string fieldString in fieldStrings)
        {
            yield return T.Parse(fieldString, default);
        }
    }

    public static IEnumerable<string> ToStringFields<T>(this T[] fields)
        where T : IFormattable
    {
        foreach(var field in fields)
        {
            yield return field.ToString(default, default);
        }
    }
}

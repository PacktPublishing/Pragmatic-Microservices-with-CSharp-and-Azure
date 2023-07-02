using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System.Text.Json;

namespace Codebreaker.Data.Cosmos.Converters;

internal class FieldValueValueConverter : ValueConverter<IDictionary<string, IEnumerable<string>>, string>
{
    // an expression tree may not contain a call or invocation that uses optional arguments
    static string GetJson(IDictionary<string, IEnumerable<string>> values)
    {
        return JsonSerializer.Serialize(values);
    }

    static IDictionary<string, IEnumerable<string>> GetDictionary(string json)
    {
        return JsonSerializer.Deserialize<IDictionary<string, IEnumerable<string>>>(json) ?? new Dictionary<string, IEnumerable<string>>();
    }

    public FieldValueValueConverter() : base(
        convertToProviderExpression: v => GetJson(v),
        convertFromProviderExpression: v => GetDictionary(v))
    { }
}

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System.Text.Json;

namespace Codebreaker.Data.Cosmos.Utilities;

internal class FieldValueValueConverter : ValueConverter<IDictionary<string, IEnumerable<string>>, string>
{
    static string GetJson(IDictionary<string, IEnumerable<string>> values) => 
        JsonSerializer.Serialize(values);

    static IDictionary<string, IEnumerable<string>> GetDictionary(string json) => 
        JsonSerializer.Deserialize<IDictionary<string, IEnumerable<string>>>(json) ?? new Dictionary<string, IEnumerable<string>>();

    public FieldValueValueConverter() : base(
        convertToProviderExpression: v => GetJson(v),
        convertFromProviderExpression: v => GetDictionary(v))
    { }
}

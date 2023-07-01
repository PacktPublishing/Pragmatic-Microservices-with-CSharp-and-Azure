using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Codebreaker.Data.SqlServer.Converters;

public class ColorFieldCollectionConverter : ValueConverter<IEnumerable<ColorField>, string>
{
    public ColorFieldCollectionConverter(Expression<Func<IEnumerable<ColorField>, string>> convertToProviderExpression, Expression<Func<string, IEnumerable<ColorField>>> convertFromProviderExpression, ConverterMappingHints? mappingHints = null) :
        base(coll => ColorFieldCollectionToString(coll), s => StringToColorFieldCollection(s), mappingHints)
    {
    }

    private static string ColorFieldCollectionToString(IEnumerable<ColorField> coll) =>
        string.Join("#", coll.Select(cf => cf.ToString()));

    private static IEnumerable<ColorField> StringToColorFieldCollection(string result) =>
        result.Split("#").Select(s => ColorField.Parse(s)).ToArray();
}

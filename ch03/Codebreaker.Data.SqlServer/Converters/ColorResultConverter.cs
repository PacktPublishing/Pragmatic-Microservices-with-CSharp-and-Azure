using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Codebreaker.Data.SqlServer.Converters;

public class ColorResultConverter : ValueConverter<ColorResult, string>
{
    public ColorResultConverter() :
        base(cr => MapColorResultToString(cr), s => MapStringToColorResult(s), null)
    {

    }

    public ColorResultConverter(ConverterMappingHints? mappingHints = default) :
        base(cr => MapColorResultToString(cr), s => MapStringToColorResult(s), mappingHints)
    {
    }

    private static string MapColorResultToString(ColorResult r) => r.ToString();
    private static ColorResult MapStringToColorResult(string s) => Enum.Parse<ColorResult>(s);
}

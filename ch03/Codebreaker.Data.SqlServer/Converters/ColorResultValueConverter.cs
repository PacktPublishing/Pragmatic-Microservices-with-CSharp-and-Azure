using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Codebreaker.Data.SqlServer.Converters;

public class ColorResultValueConverter : ValueConverter<ColorResult, int>
{
    public ColorResultValueConverter(ConverterMappingHints? mappingHints = null) :
        base(cr => cr.Correct << 4 + cr.WrongPosition, n => new ColorResult(n >> 4, n & 0b1111))
    {
    }
}

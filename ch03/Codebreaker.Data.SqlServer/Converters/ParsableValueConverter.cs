using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Codebreaker.Data.SqlServer.Converters;

public class ParsableValueConverter<TModel> : ValueConverter<TModel, string>
    where TModel : IParsable<TModel>
{
    public ParsableValueConverter() :
        base(convertToProviderExpression: t => MapTToString(t), convertFromProviderExpression: s => MapStringToT(s))
    { }

    private static string MapTToString(TModel value) => value.ToString() ?? throw new InvalidOperationException();
    private static TModel MapStringToT(string s) => TModel.Parse(s, default);
}

namespace Codebreaker.WPF.Converters;

public class FieldValuesToColorsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IDictionary<string, string[]> data)
        {
            return data["colors"];
        }
        else
        {
            return value;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

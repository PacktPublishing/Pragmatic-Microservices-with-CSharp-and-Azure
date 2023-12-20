namespace Codebreaker.WPF.Converters;

internal class IntToEnumerableConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int count)
            throw new ArgumentException("The value needs to be an integer");

        return Enumerable.Range(0, count);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

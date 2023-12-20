namespace Codebreaker.WPF.Converters;

public class KeyPegColorNameToBrushConverter : IValueConverter
{
    public Brush BlackBrush { get; set; } = new SolidColorBrush(Colors.Black);
    public Brush WhiteBrush { get; set; } = new SolidColorBrush(Colors.White);
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "Black" => BlackBrush,
            "White" => WhiteBrush,
            _ => value
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

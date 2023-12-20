namespace Codebreaker.WPF.Converters;

internal class GameStatusToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        static Visibility GetStartVisibility(GameMode gameMode) =>
            (gameMode is not GameMode.NotRunning) ? Visibility.Collapsed : Visibility.Visible;

        static Visibility GetRunningVisibility(GameMode gameMode) =>
            (gameMode is GameMode.NotRunning) ? Visibility.Collapsed : Visibility.Visible;

        static Visibility GetCancelVisibility(GameMode gameMode) =>
            (gameMode is GameMode.Started or GameMode.MoveSet) ? Visibility.Visible : Visibility.Collapsed;

        string uiCategory = parameter?.ToString() ?? throw new InvalidOperationException("Pass a parameter to this converter");

        if (value is GameMode gameMode)
        {
            return uiCategory switch
            {
                "Start" => GetStartVisibility(gameMode),
                "Running" => GetRunningVisibility(gameMode),
                "Cancelable" => GetCancelVisibility(gameMode),
                _ => throw new InvalidOperationException("Invalid parameter value")
            };
        }
        else
        {
            throw new InvalidOperationException("GameStatusToVisibilityConverter received an incorrect value type");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

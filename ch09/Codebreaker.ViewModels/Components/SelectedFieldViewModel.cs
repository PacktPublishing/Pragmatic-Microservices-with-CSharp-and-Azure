namespace Codebreaker.ViewModels.Components;

public partial class SelectedFieldViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _value;

    public bool IsSet =>
        Value is not null && Value != string.Empty;

    public void Reset() =>
        Value = null;
}

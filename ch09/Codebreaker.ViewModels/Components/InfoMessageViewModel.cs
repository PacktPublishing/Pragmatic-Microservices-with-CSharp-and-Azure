namespace Codebreaker.ViewModels.Components;

public enum InfoMessageSeverity
{
    Info,
    Success,
    Warning,
    Error
}

public partial class InfoMessageViewModel(Action closeAction) : ObservableObject
{
    [ObservableProperty]
    private InfoMessageSeverity _severity;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAction))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteActionCommand))]
    private Action? _action;

    [RelayCommand]
    public void ExecuteAction() => Action?.Invoke();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAction))]
    private string? _actionText = "OK";

    public bool HasAction => ExecuteActionCommand is not null && ActionText is not null;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CloseCommand))]
    private Action _closeAction = closeAction;

    [ObservableProperty]
    private bool _isClosable;

    [RelayCommand]
    public void Close() => CloseAction();
}
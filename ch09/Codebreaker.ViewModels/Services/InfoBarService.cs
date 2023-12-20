using Codebreaker.ViewModels.Components;
using Codebreaker.ViewModels.Contracts.Services;

namespace Codebreaker.ViewModels.Services;

public class InfoBarService : IInfoBarService
{
    public ObservableCollection<InfoMessageViewModel> Messages { get; } = [];

    public InfoMessageBuilder New => new(this);

    public void Clear() =>
        Messages.Clear();
}

public class InfoMessageBuilder
{
    private readonly IInfoBarService _infoBarService;

    private readonly InfoMessageViewModel _message;

    public InfoMessageBuilder(IInfoBarService infoBarService)
    {
        _infoBarService = infoBarService;
        _message = new(() => infoBarService.Messages.Remove(_message!))
        {
            IsClosable = true
        };
    }

    public InfoMessageBuilder WithSeverity(InfoMessageSeverity severity)
    {
        _message.Severity = severity;
        return this;
    }

    public InfoMessageBuilder IsInformationMessage() => WithSeverity(InfoMessageSeverity.Info);

    public InfoMessageBuilder IsWarningMessage() => WithSeverity(InfoMessageSeverity.Warning);

    public InfoMessageBuilder IsErrorMessage() => WithSeverity(InfoMessageSeverity.Error);

    public InfoMessageBuilder IsSuccessMessage() => WithSeverity(InfoMessageSeverity.Success);

    public InfoMessageBuilder WithTitle(string title)
    {
        _message.Title = title;
        return this;
    }

    public InfoMessageBuilder WithMessage(string message)
    {
        _message.Message = message;
        return this;
    }

    public InfoMessageBuilder WithAction(Action<InfoMessageViewModel> action, string? text = null)
    {
        _message.Action = () => action(_message);

        if (text is not null)
            _message.ActionText = text;

        return this;
    }

    public InfoMessageBuilder WithAction(Action action, string? text = null)
    {

        _message.Action = action;

        if (text is not null)
            _message.ActionText = text;

        return this;
    }

    public InfoMessageBuilder IsClosable(bool closeable = true)
    {
        _message.IsClosable = closeable;
        return this;
    }

    public InfoMessageViewModel Show()
    {
        _infoBarService.Messages.Add(_message);
        return _message;
    }
}
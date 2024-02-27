using CodeBreaker.Blazor.UI.Services.Dialog;
using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;

public partial class CodeBreakerDialog : IDisposable
{
    public bool ModalHidden { get; set; } = true;
    private string _title = string.Empty;
    private RenderFragment? _dialogContent;
    private List<DialogActionContext> _dialogActions = [];

    [Inject]
    private IDialogService CodeBreakerDialogService { get; set; } = default!;

    protected override void OnInitialized()
    {
        CodeBreakerDialogService.OnShowDialog += ShowDialog;
    }

    private void ShowDialog(object? sender, DialogContext context)
    {
        _title = context.DialogTitle;
        _dialogContent = __builder =>
        {
            __builder.OpenComponent(0, context.ComponentType);
            if (context.Parameters?.Count > 0)
            {
                foreach (var param in context.Parameters)
                {
                    __builder.AddAttribute(1, param.Key, param.Value);
                }
            }
            __builder.CloseComponent();
        };
        _dialogActions = context.Actions;
        ModalHidden = false;
        StateHasChanged();
    }

    private void CallAction(Action action)
    {
        action.Invoke();
        CloseDialog();
    }

    private void CloseDialog()
    {
        _dialogContent = null;
        ModalHidden = true;
        StateHasChanged();
    }
    public void Dispose()
    {
        CodeBreakerDialogService.OnShowDialog -= ShowDialog;
    }
}

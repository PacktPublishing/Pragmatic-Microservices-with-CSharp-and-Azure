namespace CodeBreaker.Blazor.UI.Services.Dialog;

public class DialogService : IDialogService
{
    public event EventHandler<DialogContext>? OnShowDialog;

    public void ShowDialog(DialogContext context)
    {
        OnShowDialog?.Invoke(this, context);
    }
}

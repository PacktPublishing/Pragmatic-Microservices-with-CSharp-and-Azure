namespace Codebreaker.WPF.Services;

public class WPFDialogService : IDialogService
{
    public Task ShowMessageAsync(string message)
    {
        MessageBox.Show(message);
        return Task.CompletedTask;
    }
}

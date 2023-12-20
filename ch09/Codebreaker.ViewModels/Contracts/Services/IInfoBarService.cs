using Codebreaker.ViewModels.Components;

namespace Codebreaker.ViewModels.Contracts.Services;

public interface IInfoBarService
{
    ObservableCollection<InfoMessageViewModel> Messages { get; }

    InfoMessageBuilder New { get; }

    void Clear();
}
namespace Codebreaker.ViewModels.Contracts.Services;

public interface INavigationService
{
    bool CanGoBack { get; }

    ValueTask<bool> NavigateToAsync(string key, object? parameter = null, bool clearNavigation = false);

    ValueTask<bool> GoBackAsync();
}
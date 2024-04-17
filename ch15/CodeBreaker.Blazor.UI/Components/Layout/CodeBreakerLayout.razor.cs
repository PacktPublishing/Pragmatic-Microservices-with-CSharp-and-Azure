using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components
{
    public partial class CodeBreakerLayout : ComponentBase, IDisposable
    {
        private bool _isRoot;
        private readonly bool _isDark;
        private bool _drawerOpen = false;
        private readonly string? _currentTheme;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public string Label { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment ToolbarContent { get; set; } = default!;

        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;

        [Parameter]
        public RenderFragment DrawerContent { get; set; } = default!;

        public void DrawerToggle() => _drawerOpen = !_drawerOpen;

        protected override void OnInitialized()
        {

            _isRoot = IsRootUrl();
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;
            base.OnInitialized();
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/");
        }

        private void NavigationManager_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            _isRoot = IsRootUrl();
            StateHasChanged();
        }

        private bool IsRootUrl()
        {
            return NavigationManager.Uri == NavigationManager.BaseUri;
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }
    }
}

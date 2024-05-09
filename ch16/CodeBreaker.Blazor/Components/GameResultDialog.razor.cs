using CodeBreaker.Blazor.Models;
using CodeBreaker.Blazor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CodeBreaker.Blazor.Components
{
    public partial class GameResultDialog
    {
        [Inject]
        private IStringLocalizer<Resource> Loc { get; init; } = default!;

        [Parameter]
        public GameMode GameMode { get; set; }
        [Parameter]
        public string Username { get; set; } = string.Empty;
    }
}

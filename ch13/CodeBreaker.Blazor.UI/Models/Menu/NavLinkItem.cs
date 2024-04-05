using CodeBreaker.Blazor.UI.Models.Icon;
using Microsoft.AspNetCore.Components.Routing;

namespace CodeBreaker.Blazor.UI.Models.Menu;

public record NavLinkItem(string Label, string Href, NavLinkMatch Match, CodeBreakerIcon Icon);

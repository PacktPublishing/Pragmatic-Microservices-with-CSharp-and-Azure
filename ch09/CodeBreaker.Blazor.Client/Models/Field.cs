namespace CodeBreaker.Blazor.Client.Models;

internal partial class Field
{
    public string? Color { get; set; }

    public string? Shape { get; set; }

    public bool Selected { get; set; }

    public bool CanDrop { get; set; }
}
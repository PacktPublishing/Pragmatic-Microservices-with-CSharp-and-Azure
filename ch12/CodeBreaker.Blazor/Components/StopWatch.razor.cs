using Microsoft.AspNetCore.Components;
using System.Timers;

namespace CodeBreaker.Blazor.Components;

public partial class StopWatch : IDisposable
{
    [Parameter]
    public bool Running { get; set; } = false;

    [Parameter]
    public EventCallback<TimeSpan> StopTimeChanged { get; set; }

    private string _currentTime = string.Empty;
    private TimeSpan _timespan = TimeSpan.Zero;
    private System.Timers.Timer? _timer;

    protected override async Task OnInitializedAsync()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!Running && _timer!.Enabled)
        {
            _timespan = TimeSpan.Zero;
            _timer.Stop();
            await StopTimeChanged.InvokeAsync(_timespan);
        }
        else if (Running && !_timer!.Enabled)
        {
            _timer.Start();
        }
        await base.OnParametersSetAsync();
    }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        await InvokeAsync(() =>
        {
            _timespan = _timespan.Add(TimeSpan.FromSeconds(1));
            _currentTime = _timespan.ToString(@"mm\:ss");
            StateHasChanged();
        });
    }

    public void Dispose() => _timer?.Dispose();
}

using System.Diagnostics.Metrics;

namespace Codebreaker.GameAPIs.Tests;

internal sealed class TestMeterFactory : IMeterFactory
{
    public List<Meter> Meters { get; } = [];

    public Meter Create(MeterOptions options)
    {
        Meter meter = new(options.Name, options.Version, Array.Empty<KeyValuePair<string, object?>>(), scope: this);
        Meters.Add(meter);
        return meter;
    }

    public void Dispose()
    {
        foreach (var meter in Meters)
        {
            meter.Dispose();
        }

        Meters.Clear();
    }
}

using Codebreaker.GameAPIs.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

Console.WriteLine("Test client - wait for service");
Console.ReadLine();

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5130/livesubscribe")
    .Build();

connection.On<GameSummary>("GameCompleted", (GameSummary summary) =>
{
    Console.WriteLine($"Game {summary.Id} completed");
});

await connection.StartAsync();

// subscribe
await connection.InvokeAsync("RegisterGameCompletions", "Game6x4");

using HttpClient client = new();
client.BaseAddress = new Uri("http://localhost:5130");

for (int i = 0; i < 10; i++)
{
    await Task.Delay(100);
    Console.WriteLine("sending a game");
    GameSummary summary = new(Guid.NewGuid(), "Game6x4", "Test", true, true, DateTime.Now, TimeSpan.FromSeconds(10));
    await client.PostAsJsonAsync("/live/game", summary);
}

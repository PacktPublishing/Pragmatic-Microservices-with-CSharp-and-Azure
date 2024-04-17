using Codebreaker.GameAPIs.Client.Models;
using Codebreaker.Grpc;

using Google.Protobuf.Collections;

using Grpc.Core;

using System.Diagnostics;

namespace CodeBreaker.Bot;

public class GrpcGamesClient(GrpcGame.GrpcGameClient client, ILogger<GrpcGamesClient> logger) : IGamesClient
{
    internal const string ActivitySourceName = "Codebreaker.GameAPIs.GRPCClient";
    internal const string Version = "1.0.0";
    internal static ActivitySource ActivitySource { get; } = new ActivitySource(ActivitySourceName, Version);

    public async Task<(Guid Id, int NumberCodes, int MaxMoves, IDictionary<string, string[]> FieldValues)> StartGameAsync(GameType gameType, string playerName, CancellationToken cancellationToken = default)
    {

        using Activity? activity = ActivitySource.StartActivity("StartGameAsync", ActivityKind.Client);
        try
        {
            CreateGameRequest request = new()
            {
                GameType = gameType.ToString(),
                PlayerName = playerName
            };
            var response = await client.CreateGameAsync(request, cancellationToken: cancellationToken);
            Guid id = Guid.Parse(response.Id);
            Dictionary<string, string[]> fieldValues = ConvertFieldValues(response.FieldValues);

            return (id, response.NumberCodes, response.MaxMoves, fieldValues);
        }
        catch (RpcException ex)
        {
            logger.Error(ex, ex.Message);
            throw;
        }
    }

    public async Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid id, string playerName, GameType gameType, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default)
    {
        SetMoveRequest request = new()
        {
            Id = id.ToString(),
            GameType = gameType.ToString(),
            MoveNumber = moveNumber,
            End = false
        };
        request.GuessPegs.AddRange(guessPegs);
        var response = await client.SetMoveAsync(request, cancellationToken: cancellationToken);
        return (response.Results.ToArray(), response.Ended, response.IsVictory);
    }

    public Task CancelGameAsync(Guid id, string playerName, GameType gameType, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<GameInfo?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            GetGameRequest request = new()
            {
                Id = id.ToString()
            };
            var response = await client.GetGameAsync(request, cancellationToken: cancellationToken);
            return ToGameInfo(response);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public Task<IEnumerable<GameInfo>> GetGamesAsync(GamesQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static Dictionary<string, string[]> ConvertFieldValues(MapField<string, FieldNames> fieldValues)
    {
        Dictionary<string, string[]> convertedFieldValues = [];
        foreach (var pair in fieldValues)
        {
            convertedFieldValues.Add(pair.Key, [.. pair.Value.Values]);
        }
        return convertedFieldValues;
    }

    private static GameInfo ToGameInfo(GetGameResponse response)
    {
        Guid id = Guid.Parse(response.Id);
        string[] codes = [.. response.Codes];
        IDictionary<string, IEnumerable<string>> fieldValues = 
            response.FieldValues.ToDictionary(
                ks => ks.Key, 
                es => es.Value.Values.AsEnumerable());
        
        return new GameInfo(
            id,
            response.GameType,
            response.PlayerName,
            response.StartTime.ToDateTime(),
            response.NumberCodes,
            response.MaxMoves)
        {
            FieldValues = fieldValues,
            Codes = codes
        };
    }
}

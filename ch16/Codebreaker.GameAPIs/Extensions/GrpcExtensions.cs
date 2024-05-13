using Codebreaker.Grpc;

using GrpcCreateGameResponse = Codebreaker.Grpc.CreateGameResponse;
using GrpcGetGameResponse = Codebreaker.Grpc.GetGameResponse;
using GrpcSetMoveResponse = Codebreaker.Grpc.SetMoveResponse;
namespace Codebreaker.GameAPIs.Extensions;

internal static class GrpcExtensions
{
    public static GrpcCreateGameResponse ToGrpcCreateGameResponse(this Game game)
    {
        string id = game.Id.ToString();

        GrpcCreateGameResponse response = new()
        {
            Id = id,
            GameType = game.GameType,
            PlayerName = game.PlayerName,
            NumberCodes = game.NumberCodes,
            MaxMoves = game.MaxMoves
        };

        foreach (var pair in game.FieldValues)
        {
            FieldNames values = new();
            values.Values.Add(pair.Value);
            response.FieldValues.Add(pair.Key, values);
        }

        return response;
    }

    public static GrpcSetMoveResponse ToGrpcSetMoveResponse(this Game game, Models.Move move)
    {
        GrpcSetMoveResponse response = new()
        {
            Id = game.Id.ToString(),
            GameType = game.GameType,
            MoveNumber = move.MoveNumber,
            Ended = game.HasEnded(),
            IsVictory = game.IsVictory,
        };
        response.Results.AddRange([.. move.KeyPegs]);
        return response;
    }

    public static GrpcGetGameResponse ToGrpcGetGameResponse(this Game game)
    {
        GrpcGetGameResponse response = new()
        {
            Id = game.Id.ToString(),
            GameType = game.GameType,
            PlayerName = game.PlayerName,
            PlayerIsAuthenticated = game.PlayerIsAuthenticated,
            StartTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(game.StartTime),
            EndTime = game.EndTime is null ? null : Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(game.EndTime.Value),
            Duration = game.Duration is null ? null : Google.Protobuf.WellKnownTypes.Duration.FromTimeSpan(game.Duration.Value),
            LastMoveNumber = game.LastMoveNumber,
            NumberCodes = game.NumberCodes,
            MaxMoves = game.MaxMoves,
            IsVictory = game.IsVictory
        };

        foreach (var pair in game.FieldValues)
        {
            FieldNames values = new();
            values.Values.Add(pair.Value);
            response.FieldValues.Add(pair.Key, values);
        }


        return response;
    }
}

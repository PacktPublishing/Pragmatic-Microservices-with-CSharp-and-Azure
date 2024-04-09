using Codebreaker.Grpc;

using Grpc.Core;

namespace Codebreaker.GameAPIs.Endpoints;

public class GrpcGameEndpoints(IGamesService gamesService, ILogger<GrpcGameEndpoints> logger) : GrpcGame.GrpcGameBase
{
    public override async Task<Grpc.CreateGameResponse> CreateGame(Grpc.CreateGameRequest request, ServerCallContext context)
    {
        logger.GameStart(request.GameType);
        Game game = await gamesService.StartGameAsync(request.GameType, request.PlayerName);
        return game.ToGrpcCreateGameResponse();
    }

    public override async Task<SetMoveResponse> SetMove(SetMoveRequest request, ServerCallContext context)
    {
        Guid id = Guid.Parse(request.Id);
        string[] guesses = request.GuessPegs.ToArray();
        (Game game, Models.Move move) = await gamesService.SetMoveAsync(id, request.GameType, guesses, request.MoveNumber);
        
        return game.ToGrpcSetMoveResponse(move);
    }

    public override async Task<GetGameResponse> GetGame(GetGameRequest request, ServerCallContext context)
    {
        Guid id = Guid.Parse(request.Id);
        Game? game = await gamesService.GetGameAsync(id);
        if (game is null) return new GetGameResponse() { Id = Guid.Empty.ToString() };
        return game.ToGrpcGetGameResponse();
    }
}

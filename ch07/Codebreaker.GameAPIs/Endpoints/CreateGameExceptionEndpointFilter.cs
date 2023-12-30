namespace Codebreaker.GameAPIs.Endpoints;

public class CreateGameExceptionEndpointFilter(ILogger<CreateGameExceptionEndpointFilter> logger) : IEndpointFilter
{
    private readonly ILogger _logger = logger;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        CreateGameRequest request = context.GetArgument<CreateGameRequest>(1);
        try
        {
            return await next(context);
        }
        catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.InvalidGameType)
        {
            _logger.LogWarning("game type {gametype} not found", request.GameType);
            return Results.BadRequest("Gametype does not exist");
        }
    }
}

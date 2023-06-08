using Codebreaker.GameAPIs.Exceptions;

namespace Codebreaker.GameAPIs.Endpoints;

public class CreateGameExceptionEndpointFilter : IEndpointFilter
{
    private readonly ILogger _logger;
    public CreateGameExceptionEndpointFilter(ILogger<CreateGameExceptionEndpointFilter> logger)
    {
        _logger = logger;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        CreateGameRequest request = context.GetArgument<CreateGameRequest>(1);
        try
        {
            return await next(context);
        }
        catch (GameTypeNotFoundException)
        {
            _logger.LogWarning("game type {gametype} not found", request.GameType);
            return Results.BadRequest("Gametype does not exist");
        }
    }
}

namespace Codebreaker.GameAPIs.Endpoints;

public class ValidatePlayernameEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        CreateGameRequest request = context.GetArgument<CreateGameRequest>(0);
        if (request.PlayerName.Length < 4)
        {
            return Results.BadRequest("Player name must be at least 4 characters long");
        }
        return await next(context);
    }
}

using Codebreaker.GameAPIs.Errors;
using Codebreaker.GameAPIs.Exceptions;

namespace Codebreaker.GameAPIs.Endpoints;

public class SetMoveExceptionHandlerFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next.Invoke(context);
        }
        catch (ArgumentException ex) when (ex.HResult is <= 4200 and >= 400)
        {
            return ex.HResult switch
            {
                4200 => TypedResults.BadRequest(new InvalidGameMoveError("Invalid number of guesses received")),
                4300 => TypedResults.BadRequest(new InvalidGameMoveError("Invalid move number received")),
                4400 => TypedResults.BadRequest(new InvalidGameMoveError("Invalid guess values received!")),
                _ => TypedResults.BadRequest(new InvalidGameMoveError("Invalid move received!")),
            };
        }
        catch (GameNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}

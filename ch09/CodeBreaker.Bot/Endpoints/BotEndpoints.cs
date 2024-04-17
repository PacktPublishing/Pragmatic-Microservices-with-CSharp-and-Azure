using Microsoft.AspNetCore.Http.HttpResults;

namespace CodeBreaker.Bot.Endpoints;

public static class BotEndpoints
{
    public static void MapBotEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/bot")
            .WithTags("Bot API");

        group.MapPost("/session", Results<BadRequest, Accepted<Guid>> (
            CodeBreakerTimer timer,
            int count = 3,
            int delay = 10,
            int thinkTime = 3) =>
        {
            Guid id;

            try
            {
                id = timer.Start(delay, count, thinkTime);
            }
            catch (ArgumentOutOfRangeException)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Accepted($"/games/{id}", id);
        })
        .WithName("CreateBot")
        .WithSummary("Starts a bot playing one or more games")
        .WithOpenApi(x =>
        {
            x.Parameters[0].Description = "The number of games to play.";
            x.Parameters[1].Description = "The delay between the games (seconds).";
            x.Parameters[2].Description = "The think time between game moves (seconds).";
            return x;
        });

        group.MapGet("/session", () =>
        {
            IEnumerable<StatusInfo> results = CodeBreakerTimer.GetAllStatuses();

            return TypedResults.Ok(results);
        })
        .WithName("GetSessions")
        .WithSummary("Gets the statuses of all sessions")
        .WithOpenApi();

        group.MapGet("/session/{id}", Results<Ok<StatusInfo>, BadRequest<string>, NotFound> (Guid id) =>
        {
            StatusInfo result;

            try
            {
                result = CodeBreakerTimer.GetStatus(id);
            }
            catch (ArgumentException)
            {
                return TypedResults.BadRequest("Invalid id");
            }
            catch (BotNotFoundException)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result);
        })
        .WithName("GetSession")
        .WithSummary("Gets the status of a bot")
        .WithOpenApi(x =>
        {
            x.Parameters[0].Description = "The id of the bot";
            return x;
        });

        group.MapDelete("/session/{id}", Results<NoContent, NotFound, BadRequest<string>> (Guid id) =>
        {
            try
            {
                CodeBreakerTimer.Stop(id);
            }
            catch (ArgumentException)
            {
                return TypedResults.BadRequest("Invalid id");
            }
            catch (BotNotFoundException)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        })
        .WithName("StopBot")
        .WithSummary("Stops the bot with the given id")
        .WithOpenApi(x =>
        {
            x.Parameters[0].Description = "The id of the bot";
            return x;
        });
    }
}

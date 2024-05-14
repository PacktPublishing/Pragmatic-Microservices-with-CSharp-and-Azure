using Codebreaker.Ranking.Data;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Ranking.Endpoints;

public static class RankingEndpoints
{
    public static void MapRankingEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/ranking")
            .WithTags("Ranking API");

        group.MapGet("/{day}", async (DateOnly day, IDbContextFactory<RankingsContext> factory) =>
        {
            using var context = await factory.CreateDbContextAsync();
            var summaries = await context.GetGameSummariesByDayAsync(day);

            return TypedResults.Ok(summaries);
        });
    }
}

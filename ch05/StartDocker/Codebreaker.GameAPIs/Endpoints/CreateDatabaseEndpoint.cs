using Microsoft.AspNetCore.Http.HttpResults;

namespace Codebreaker.GameAPIs.Endpoints;

public static class CreateDatabaseEndpoint
{
    public static void MapCreateDatabaseEndpoints(this IEndpointRouteBuilder routes, ILogger logger)
    {
        routes.MapPost("/updatesql", async Task<Results<Ok<string>, UnprocessableEntity<string>>> (IGamesRepository repository) =>
        {
            if (repository is GamesSqlServerContext context)
            {
                await context.Database.MigrateAsync();
                return TypedResults.Ok("Database updated");
            }
            else
            {
                logger.LogError("/updatesql invoked, but SQL Server is not configured");
                return TypedResults.UnprocessableEntity("SQL Server is not configured");
            }
        }).WithTags("Database");
    }
}

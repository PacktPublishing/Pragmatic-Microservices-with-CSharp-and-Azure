using Codebreaker.Data.Sqlite;
using Codebreaker.GameAPIs.Data;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Codebreaker.GameAPIs.Endpoints;

public static class CreateDatabaseEndpoint
{
    public static void MapCreateDatabaseEndpoints(this IEndpointRouteBuilder routes, ILogger logger)
    {
        routes.MapPost("/createsqlite", async Task<Results<Ok<string>, UnprocessableEntity<string>>> (IGamesRepository repository) =>
        {
            if (repository is GamesSqliteContext context)
            {
                await context.Database.EnsureCreatedAsync();
                return TypedResults.Ok("Database updated");
            }
            else
            {
                logger.LogError("/createsqldb invoked, but Sqlite is not configured");
                return TypedResults.UnprocessableEntity("Sqlite is not configured");
            }
        }).WithTags("Database");
    }
}

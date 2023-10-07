using Codebreaker.Data.Sqlite;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Codebreaker.GameAPIs.Endpoints;

// using AOT with EF Core 8.0, migrations and creating the database is not possible.
// We use a migrations bundle to create the database.
//public static class CreateDatabaseEndpoint
//{
//    public static void MapCreateDatabaseEndpoints(this IEndpointRouteBuilder routes, ILogger logger)
//    {
//        routes.MapPost("/createdb", async Task<Results<Ok<string>, UnprocessableEntity<string>>> (IGamesRepository repository) =>
//        {
//            if (repository is GamesSqliteContext context)
//            {
//                await context.Database.EnsureCreatedAsync();
//                return TypedResults.Ok("Database created");
//            }
//            else
//            {
//                logger.LogError("/updatesql invoked, but SQL Server is not configured");
//                return TypedResults.UnprocessableEntity("SQL Server is not configured");
//            }
//        }).WithTags("Database");
//    }
//}

namespace Codebreaker.GameAPIs.Endpoints;

// TODO: only used with Cosmos emulator issues
public static class CosmosEndpoint
{
    public static void MapCreateCosmosEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cosmos")
        .WithTags("Cosmos API");

        group.MapGet("/create", async (
            IGamesRepository repository,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            try
            {
                if (repository is GamesCosmosContext gamesCosmosContext)
                {
                    bool created = await gamesCosmosContext.Database.EnsureCreatedAsync();
                    return TypedResults.Ok($"Created: {created}");
                }
                return TypedResults.Ok($"not cosmos");
            }
            catch (Exception ex)
            {
                return TypedResults.Ok(ex.ToString());
            }
        });
    }
}
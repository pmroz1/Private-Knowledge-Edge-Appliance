namespace API.Routes;

public static class Ask
{
    public static void MapAsk(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost(
                "/chat",
                async (HttpContext context) =>
                {
                    return Results.Ok();
                }
            )
            .WithName("Chat")
            .WithTags("Chat")
            .WithOpenApi();
    }
}

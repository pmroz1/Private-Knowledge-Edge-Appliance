namespace API.Routes;

using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public static class Ask
{
    public static void MapAsk(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost(
                "/chat",
                async (
                    HttpContext context,
                    IAskService askService,
                    [FromBody] AskRequest request
                ) =>
                {
                    if (string.IsNullOrWhiteSpace(request.Question))
                    {
                        return Results.BadRequest("Question is required.");
                    }

                    var answer = await askService.AskAsync(request.Question);
                    return Results.Ok(new { Answer = answer });
                }
            )
            .WithName("Chat")
            .WithTags("Chat")
            .WithOpenApi();
    }
}

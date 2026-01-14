using API.Services.Interfaces;

namespace API.Routes;

public static class Docs
{
    public static void MapDocs(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/upload", Upload)
            .WithName("Upload")
            .WithTags("Upload")
            .DisableAntiforgery();
    }

    private static async Task<IResult> Upload(
        IFormFile file,
        IPdfService pdfService,
        IQdrantService qdrantService
    )
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var text = await pdfService.ExtractTextAsync(stream);

            var metadata = new Dictionary<string, object>
            {
                { "filename", file.FileName },
                { "uploadDate", DateTime.UtcNow },
            };

            await qdrantService.UpsertTextAsync("documents", text, metadata);

            return Results.Ok(
                new { Text = text, Message = "Document processed and uploaded to Qdrant." }
            );
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}

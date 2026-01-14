namespace API.Startup;

using API.Services;
using API.Services.Interfaces;

public static class StartupExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPdfService, PdfService>();
    }

    // private static void RegisterQdrant(this WebApplicationBuilder builder)
    // {
    //     app.AddQdrantClient();
    //     return app;
    // }
}

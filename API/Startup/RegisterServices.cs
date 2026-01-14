namespace API.Startup;

using API.Services;
using API.Services.Interfaces;

public static class StartupExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPdfService, PdfService>();

        // Configuration
        var qdrantConfig = builder.Configuration.GetSection("Qdrant");
        var ollamaConfig = builder.Configuration.GetSection("Ollama");

        // Qdrant Client
        builder.Services.AddSingleton<Qdrant.Client.QdrantClient>(sp =>
        {
            var host = qdrantConfig["Host"] ?? "localhost";
            var port = qdrantConfig.GetValue<int>("Port", 6333);
            var apiKey = qdrantConfig["ApiKey"];
            var https = false; // Set to true if using HTTPS
            return new Qdrant.Client.QdrantClient(host, port, https, apiKey);
        });

        // Ollama Client
        builder.Services.AddSingleton<OllamaSharp.IOllamaApiClient>(sp =>
        {
            var baseUrl = ollamaConfig["BaseUrl"] ?? "http://localhost:11434";
            return new OllamaSharp.OllamaApiClient(baseUrl);
        });

        // Services
        builder.Services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
        builder.Services.AddSingleton<IQdrantService, QdrantService>();
    }

    // private static void RegisterQdrant(this WebApplicationBuilder builder)
    // {
    //     app.AddQdrantClient();
    //     return app;
    // }
}

using API.Services.Interfaces;
using OllamaSharp;

namespace API.Services;

public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly IOllamaApiClient _ollamaClient;
    private readonly IConfiguration _configuration;

    public OllamaEmbeddingService(IOllamaApiClient ollamaClient, IConfiguration configuration)
    {
        _ollamaClient = ollamaClient;
        _configuration = configuration;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var model = _configuration["Ollama:Model"] ?? "nomic-embed-text";
        var request = new OllamaSharp.Models.EmbedRequest
        {
            Model = model,
            Input = new List<string> { text },
        };
        var response = await _ollamaClient.EmbedAsync(request);
        return response.Embeddings.First().ToArray();
    }
}

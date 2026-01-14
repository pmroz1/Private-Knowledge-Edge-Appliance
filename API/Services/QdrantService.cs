using API.Services.Interfaces;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace API.Services;

public class QdrantService : IQdrantService
{
    private readonly QdrantClient _qdrantClient;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<QdrantService> _logger;
    private readonly int _vectorSize;

    public QdrantService(
        QdrantClient qdrantClient,
        IEmbeddingService embeddingService,
        IConfiguration configuration,
        ILogger<QdrantService> logger
    )
    {
        _qdrantClient = qdrantClient;
        _embeddingService = embeddingService;
        _logger = logger;
        _vectorSize = configuration.GetValue<int>("Qdrant:VectorSize", 768);
    }

    public async Task UpsertTextAsync(
        string collectionName,
        string text,
        Dictionary<string, object> metadata
    )
    {
        await EnsureCollectionAsync(collectionName);

        var chunks = ChunkText(text, 1000);

        var points = new List<PointStruct>();

        foreach (var chunk in chunks)
        {
            try
            {
                var embedding = await _embeddingService.GetEmbeddingAsync(chunk);

                var point = new PointStruct
                {
                    Id = Guid.NewGuid(),
                    Vectors = embedding,
                    Payload =
                    {
                        ["text"] = chunk,
                        ["metadata"] = System.Text.Json.JsonSerializer.Serialize(metadata),
                    },
                };

                foreach (var kvp in metadata)
                {
                    point.Payload.Add(kvp.Key, kvp.Value?.ToString() ?? string.Empty);
                }

                points.Add(point);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embedding for chunk");
            }
        }

        if (points.Any())
        {
            await _qdrantClient.UpsertAsync(collectionName, points);
            _logger.LogInformation(
                $"Upserted {points.Count} points to collection {collectionName}"
            );
        }
    }

    public async Task<List<string>> SearchAsync(
        string collectionName,
        float[] vector,
        int limit = 3
    )
    {
        await EnsureCollectionAsync(collectionName);

        var hits = await _qdrantClient.SearchAsync(collectionName, vector, limit: (ulong)limit);

        return hits.Select(h => h.Payload["text"].StringValue).ToList();
    }

    private async Task EnsureCollectionAsync(string collectionName)
    {
        var collections = await _qdrantClient.ListCollectionsAsync();
        if (!collections.Contains(collectionName))
        {
            await _qdrantClient.CreateCollectionAsync(
                collectionName,
                new VectorParams { Size = (ulong)_vectorSize, Distance = Distance.Cosine }
            );
        }
    }

    private List<string> ChunkText(string text, int chunkSize)
    {
        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += chunkSize)
        {
            chunks.Add(text.Substring(i, Math.Min(chunkSize, text.Length - i)));
        }
        return chunks;
    }
}

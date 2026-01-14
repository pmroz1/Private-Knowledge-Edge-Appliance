namespace API.Services.Interfaces;

public interface IQdrantService
{
    Task UpsertTextAsync(string collectionName, string text, Dictionary<string, object> metadata);
    Task<List<string>> SearchAsync(string collectionName, float[] vector, int limit = 3);
}

namespace API.Services.Interfaces;

public interface IQdrantService
{
    Task UpsertTextAsync(string collectionName, string text, Dictionary<string, object> metadata);
}

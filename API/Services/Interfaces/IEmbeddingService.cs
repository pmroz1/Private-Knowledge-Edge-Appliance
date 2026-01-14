namespace API.Services.Interfaces;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
}

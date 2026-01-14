using System.Text;
using API.Services.Interfaces;
using OllamaSharp;

namespace API.Services;

public class AskService : IAskService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IQdrantService _qdrantService;
    private readonly IOllamaApiClient _ollamaClient;
    private readonly IConfiguration _configuration;

    public AskService(
        IEmbeddingService embeddingService,
        IQdrantService qdrantService,
        IOllamaApiClient ollamaClient,
        IConfiguration configuration
    )
    {
        _embeddingService = embeddingService;
        _qdrantService = qdrantService;
        _ollamaClient = ollamaClient;
        _configuration = configuration;
    }

    public async Task<string> AskAsync(string question)
    {
        var embedding = await _embeddingService.GetEmbeddingAsync(question);
        var results = await _qdrantService.SearchAsync("documents", embedding, limit: 3);
        var context = string.Join("\n\n", results);

        var prompt = FormatPrompt(context, question);

        var model = _configuration["Ollama:Model"] ?? "nomic-embed-text";
        var chatModel = _configuration["Ollama:ChatModel"] ?? "llama3.2";

        var request = new OllamaSharp.Models.GenerateRequest
        {
            Model = chatModel,
            Prompt = prompt,
            Stream = false,
        };

        var sb = new StringBuilder();
        await foreach (var stream in _ollamaClient.GenerateAsync(request))
        {
            sb.Append(stream.Response);
        }
        return sb.ToString();
    }

    private string FormatPrompt(string context, string userQuestion)
    {
        return $@"You are a Senior Systems Engineer specializing in technical support and documentation analysis. 
Your goal is to provide accurate, professional, and concise answers based on the provided context.

RULES:
1. Use ONLY the provided CONTEXT to answer the question. 
2. If the answer is not contained within the context, state that you do not have enough information in the local knowledge base, but do not make up facts.
3. Keep the tone professional, technical, and objective.
4. If the context contains steps or commands, format them using Markdown code blocks.
5. Answer in English.

CONTEXT:
{context}

USER QUESTION:
{userQuestion}

TECHNICAL RESPONSE:
";
    }
}

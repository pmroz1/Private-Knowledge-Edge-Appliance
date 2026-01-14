using API.Models;

namespace API.Services.Interfaces;

public interface IAskService
{
    Task<string> AskAsync(string question);
}

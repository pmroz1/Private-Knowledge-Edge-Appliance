using System.IO;
using System.Threading.Tasks;

namespace API.Services.Interfaces;

public interface IPdfService
{
    Task<string> ExtractTextAsync(Stream pdfStream);
}

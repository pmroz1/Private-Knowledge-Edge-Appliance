using System.IO;
using System.Text;
using System.Threading.Tasks;
using API.Services.Interfaces;
using UglyToad.PdfPig;

namespace API.Services;

public class PdfService : IPdfService
{
    public Task<string> ExtractTextAsync(Stream pdfStream)
    {
        return Task.Run(() =>
        {
            using var document = PdfDocument.Open(pdfStream);
            var text = new StringBuilder();
            foreach (var page in document.GetPages())
            {
                text.Append(page.Text);
                text.Append(' '); // Add space between pages
            }
            return text.ToString().Trim();
        });
    }
}

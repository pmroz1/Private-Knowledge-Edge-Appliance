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
                var words = page.GetWords();
                foreach (var word in words)
                {
                    if (word.Text.Contains("TCPDF") || word.Text.Contains("www.tcpdf.org"))
                    {
                        continue;
                    }

                    text.Append(word.Text);
                    text.Append(' ');
                }
                text.AppendLine();
            }
            return text.ToString().Trim();
        });
    }
}

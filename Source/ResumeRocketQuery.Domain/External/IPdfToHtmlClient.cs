using System.IO;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IPdfToHtmlClient
    {
        Task<Stream> ConvertPdf(MemoryStream stream);
        Task<Stream> StripHtmlElements(Stream html);
        Task<Stream> StripSpans(Stream html);
    }
}

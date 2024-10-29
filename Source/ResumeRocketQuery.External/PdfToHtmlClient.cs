using HtmlAgilityPack;
using ResumeRocketQuery.Domain.External;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;

namespace ResumeRocketQuery.External
{
    public class PdfToHtmlClient : IPdfToHtmlClient
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public PdfToHtmlClient(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<Stream> ConvertPdf(MemoryStream stream)
        {
            var httpClient = new HttpClient();

            using (var form = new MultipartFormDataContent())
            {
                stream.Position = 0;

                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                form.Add(fileContent, "file", "uploaded.pdf"); 


                HttpResponseMessage response = await httpClient.PostAsync(_resumeRocketQueryConfigurationSettings.Pdf2HtmlUrl, form);

                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();

                return responseStream;
            }
        }

        public async Task<Stream> StripHtmlElements(Stream html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(html);

            foreach (var tag in new[]{ "style", "link", "script", "img", "head", "meta" })
            {
                var nodes = htmlDoc.DocumentNode.Descendants(tag).ToList();

                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(htmlDoc.DocumentNode.InnerHtml);

            return new MemoryStream(byteArray);
        }
    }
}

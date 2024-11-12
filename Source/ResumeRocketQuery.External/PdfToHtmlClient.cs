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

            foreach (var tag in new[]{ "style", "link", "script", "img", "head", "meta"  })
            {
                var nodes = htmlDoc.DocumentNode.Descendants(tag).ToList();

                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }

            var resultHtml = StripSpans(htmlDoc);

            byte[] byteArray = Encoding.UTF8.GetBytes(resultHtml.DocumentNode.InnerHtml);

            return new MemoryStream(byteArray);
        }

        public async Task<Stream> StripSpans(Stream html)
        {
            var htmlDoc = new HtmlDocument();

            htmlDoc.Load(html);

            var resultHtml = StripSpans(htmlDoc);

            byte[] byteArray = Encoding.UTF8.GetBytes(resultHtml.DocumentNode.InnerHtml);

            return new MemoryStream(byteArray);
        }

        private HtmlDocument StripSpans(HtmlDocument htmlDoc)
        {
            var spans = htmlDoc.DocumentNode.SelectNodes("//span");

            if (spans != null)
            {
                foreach (var span in spans)
                {
                    var innerText = span.InnerText;

                    span.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(innerText), span);
                }
            }

            return htmlDoc;
        }

    }
}

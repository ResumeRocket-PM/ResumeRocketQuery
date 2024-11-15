using HtmlAgilityPack;
using ResumeRocketQuery.Domain.External;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using System;
using System.Text.RegularExpressions;

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

        public async Task<string> StripText(Stream htmlsStream)
        {
            var strippedStream = await StripHtmlElements(htmlsStream);

            var htmlDoc = new HtmlDocument();

            htmlDoc.Load(strippedStream);

            string textContent = GetVisibleText(htmlDoc.DocumentNode);

            string alphanumericText = Regex.Replace(textContent, @"[^a-zA-Z0-9\s]", " ");

            alphanumericText = Regex.Replace(alphanumericText, @"[\r\n]+", " ");

            alphanumericText = Regex.Replace(alphanumericText, @"\s+", " "); 


            return alphanumericText;
        }

        private string GetVisibleText(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                var style = node.GetAttributeValue("style", string.Empty);
                if (style.Contains("display: none", StringComparison.OrdinalIgnoreCase) ||
                    style.Contains("visibility: hidden", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }
            }

            if (node.NodeType == HtmlNodeType.Text)
            {
                return node.InnerText;
            }

            var visibleText = string.Empty;

            foreach (var childNode in node.ChildNodes)
            {
                visibleText += GetVisibleText(childNode);
            }

            return visibleText;
        }

    }
}

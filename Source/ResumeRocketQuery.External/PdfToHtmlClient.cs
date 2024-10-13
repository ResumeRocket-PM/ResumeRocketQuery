using ResumeRocketQuery.Domain.External;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ResumeRocketQuery.External
{
    public class PdfToHtmlClient : IPdfToHtmlClient
    {
        private string _apiUrl = "http://localhost:5000/convert";

        public async Task<Stream> ConvertPdf(MemoryStream stream)
        {
            var httpClient = new HttpClient();

            using (var form = new MultipartFormDataContent())
            {
                stream.Position = 0;

                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                form.Add(fileContent, "file", "uploaded.pdf"); 


                HttpResponseMessage response = await httpClient.PostAsync(_apiUrl, form);
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();

                return responseStream;
            }
        }
    }
}

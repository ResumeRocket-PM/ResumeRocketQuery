using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using System.Text.Json;
using ResumeRocketQuery.Domain.Services;
using System.IO;
using System.Net.Http.Json;
using JetBrains.Annotations;
using System;
using System.Net.Http.Headers;

namespace ResumeRocketQuery.External
{
    public class LlamaClient : ILlamaClient
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public LlamaClient(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<string> CreateMessage(string input)
        {
            var httpClient = new HttpClient();

            var content = new StringContent(JsonSerializer.Serialize(new { message = input }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync($"{_resumeRocketQueryConfigurationSettings.LlamaClientUrl}/message", content);

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        public async Task<string> JobDetails(Stream stream, string siteName)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var multipartContent = new MultipartFormDataContent())
                {
                    multipartContent.Add(new StringContent(siteName), "site");

                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                    multipartContent.Add(fileContent, "file", "job.html");

                    HttpResponseMessage response = await client.PostAsync($"{_resumeRocketQueryConfigurationSettings.LlamaClientUrl}/get_job_details", multipartContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
        }

        public async Task<string> ParseJobPosting(string html)
        {
        var payload = new { message = html };
        var jsonContent = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(10);
            HttpResponseMessage response = await client.PostAsync($"{_resumeRocketQueryConfigurationSettings.LlamaClientUrl}/parse", content);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        }
    }
}

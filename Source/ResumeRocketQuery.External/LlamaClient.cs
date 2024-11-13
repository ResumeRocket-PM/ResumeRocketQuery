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
            var httpClient = new HttpClient();

            var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), "file", "job-posting.html" },
                { new StringContent(siteName), "site" }
            };

            HttpResponseMessage response = await httpClient.PostAsync($"{_resumeRocketQueryConfigurationSettings.LlamaClientUrl}/get_job_details", content);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
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

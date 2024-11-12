using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using System.Text.Json;
using ResumeRocketQuery.Domain.Services;

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
    }
}

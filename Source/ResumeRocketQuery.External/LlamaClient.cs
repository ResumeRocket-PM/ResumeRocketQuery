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
        private string _url = "http://localhost:5014/message/";

        public LlamaClient(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<string> CreateMessage(string input)
        {
            var httpClient = new HttpClient();

            var content = new StringContent(JsonSerializer.Serialize(new { message = input }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(_url, content);

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
    }
}

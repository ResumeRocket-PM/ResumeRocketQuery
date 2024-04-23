using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using Newtonsoft.Json;

namespace ResumeRocketQuery.Api.Tests.Helpers
{
    public class RestRequestClient
    {
        private readonly HttpClient _client;
        public RestRequestClient()
        {
            _client = new HttpClient();
        }

        public async Task<ServiceResponseGeneric<T>> SendRequest<T>(string resource, HttpMethod httpMethod, object requestBody = null, Dictionary<string, string> requestHeaders = null)
        {
            var httpRequestMessage = new HttpRequestMessage(httpMethod, resource);

            if (requestBody != null)
            {
                var request = JsonConvert.SerializeObject(requestBody);
                httpRequestMessage.Content = new StringContent(request, Encoding.UTF8, "application/json");
            }

            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            try
            {
                var response = await _client.SendAsync(httpRequestMessage);

                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ServiceResponseGeneric<T>>(responseContent);

                return result;
            }
            catch (Exception e)
            {
                //
                throw e;
            }
        }
    }
}

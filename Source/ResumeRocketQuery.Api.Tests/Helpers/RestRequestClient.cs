using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using Newtonsoft.Json;
using Azure.Core;
using Microsoft.Net.Http.Headers;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace ResumeRocketQuery.Api.Tests.Helpers
{
    public class RestRequestClient
    {
        private readonly HttpClient _client;
        public RestRequestClient()
        {
            _client = new HttpClient();
        }

        public async Task<ServiceResponseGeneric<T>> SendRequest<T>(string resource, HttpMethod httpMethod, object requestBody = null, Dictionary<string, string> requestHeaders = null, string fileUpload = null)
        {
            var httpRequestMessage = new HttpRequestMessage(httpMethod, resource);


            if (requestBody != null)
            {
                
                if (fileUpload != null)
                {
                    var content = new MultipartFormDataContent();

                    var byteArrayContent = new ByteArrayContent(File.ReadAllBytes(fileUpload));
                    byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Add(byteArrayContent, "FormFile", Path.GetFileName(fileUpload));

                    var requestBodyJson = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8,
                        "application/json");
                    content.Add(requestBodyJson, "Data");
                    
                    httpRequestMessage.Content = content;
                }
                else
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestBody),
                        Encoding.UTF8, "application/json");
                }

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

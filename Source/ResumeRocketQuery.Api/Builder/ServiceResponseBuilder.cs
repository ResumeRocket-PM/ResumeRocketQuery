using System.Collections.Generic;
using System.Net;
using ResumeRocketQuery.Domain.Api;

namespace ResumeRocketQuery.Api.Builder
{
    public class ServiceResponseBuilder : IServiceResponseBuilder
    {
        private readonly List<HttpStatusCode> _successfulStatusCodes = new List<HttpStatusCode>
        {
            HttpStatusCode.OK,
            HttpStatusCode.Created
        };

        public ServiceResponse<T> BuildServiceResponse<T>(T result, HttpStatusCode statusCode)
        {
            var success = _successfulStatusCodes.Contains(statusCode);

            var response = new ServiceResponse<T>
            {
                Result = result,
                Succeeded = success,
                ResponseMetadata = new ResponseMetadata
                {
                    HttpStatusCode = (int)statusCode
                }
            };

            return response;
        }
    }
}

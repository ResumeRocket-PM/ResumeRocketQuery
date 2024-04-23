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

        public ServiceResponseGeneric<T> BuildServiceResponse<T>(T result, HttpStatusCode statusCode)
        {
            var success = _successfulStatusCodes.Contains(statusCode);

            var response = new ServiceResponseGeneric<T>
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

        public ServiceResponse BuildServiceResponse(HttpStatusCode statusCode)
        {
            var success = _successfulStatusCodes.Contains(statusCode);

            var response = new ServiceResponse
            {
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

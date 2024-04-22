using System.Net;

namespace ResumeRocketQuery.Domain.Api
{
    public interface IServiceResponseBuilder
    {
        ServiceResponseGeneric<T> BuildServiceResponse<T>(T result, HttpStatusCode statusCode);
        ServiceResponse BuildServiceResponse(HttpStatusCode statusCode);
    }
}

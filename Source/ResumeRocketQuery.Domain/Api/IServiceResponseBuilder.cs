using System.Net;

namespace ResumeRocketQuery.Domain.Api
{
    public interface IServiceResponseBuilder
    {
        ServiceResponse<T> BuildServiceResponse<T>(T result, HttpStatusCode statusCode);
    }
}

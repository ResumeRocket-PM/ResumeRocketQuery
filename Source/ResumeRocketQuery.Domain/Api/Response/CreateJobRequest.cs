using Microsoft.AspNetCore.Http;

namespace ResumeRocketQuery.Domain.Api.Response;

public class CreateJobRequest
{
    public IFormFile FormFile { get; set; }
    public string Data { get; set; }
}
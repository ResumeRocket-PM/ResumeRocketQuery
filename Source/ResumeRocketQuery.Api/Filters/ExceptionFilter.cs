using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging; // Import this namespace
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger; // Declare a logger

        // Constructor to inject the logger
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "{Message}", context.Exception.Message);

            var response = new ServiceResponseGeneric<object>
            {
                Result = null,
                ResponseMetadata = new ResponseMetadata
                {
                    // Default response
                    HttpStatusCode = 500,
                    Exception = context.Exception.Message 
                },
                Succeeded = false
            };

            if (context.Exception is Domain.Services.ValidationException validationException)
            {
                response.ResponseMetadata.HttpStatusCode = 400; 
                response.ResponseMetadata.Exception = "One or more validation errors occurred.";
            }

            var jsonObject = JsonConvert.SerializeObject(response);

            context.HttpContext.Response.StatusCode = response.ResponseMetadata.HttpStatusCode;
            context.HttpContext.Response.ContentType = "application/json";

            await context.HttpContext.Response.WriteAsync(jsonObject);

            context.ExceptionHandled = true;
        }
    }
}

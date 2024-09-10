using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {

            var response = new ServiceResponseGeneric<object>
            {
                Result = null,
                ResponseMetadata = new ResponseMetadata
                {
                    // Default response
                    HttpStatusCode = 500,
                    Exception = context.Exception.Message // Don't expose this in production
                },
                Succeeded = false
            };

            if (context.Exception is Domain.Services.ValidationException validationException)
            {
                // Handle ValidationException separately
                response.ResponseMetadata.HttpStatusCode = 400; // Bad Request
                response.ResponseMetadata.Exception = "One or more validation errors occurred.";
            }

            // Serialize the response object to JSON
            var jsonObject = JsonConvert.SerializeObject(response);

            // Set the response status code and content type
            context.HttpContext.Response.StatusCode = response.ResponseMetadata.HttpStatusCode;
            context.HttpContext.Response.ContentType = "application/json";

            // Write the serialized JSON to the response body
            await context.HttpContext.Response.WriteAsync(jsonObject);

            // Mark the exception as handled
            context.ExceptionHandled = true;
        }
    }
}


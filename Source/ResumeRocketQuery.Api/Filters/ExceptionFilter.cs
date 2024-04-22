using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

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
                    HttpStatusCode = 500,
                    Exception = context.Exception.Message
                },
                Succeeded = false
            };

            var jsonObject = JsonConvert.SerializeObject(response);

            context.HttpContext.Response.StatusCode = 500;
            context.HttpContext.Response.ContentType = "application/json";

            await context.HttpContext.Response.WriteAsync(jsonObject);

            await context.HttpContext.Response.CompleteAsync();
        }
    }
}


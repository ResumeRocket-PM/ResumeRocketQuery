using System.Linq;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace ResumeRocketQuery.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState;

                var response = new ServiceResponse<object>
                {
                    Result = null,
                    ResponseMetadata = new ResponseMetadata
                    {
                        HttpStatusCode = 400,
                        Exception = null,
                        ValidationErrors = errors.Select(x => new ValidationError
                        {
                            Property = x.Key,
                            Message = string.Join(",", x.Value.Errors.Select(y => y.ErrorMessage).ToList())
                        }).ToList()
                    },
                    Succeeded = false
                };

                var jsonObject = JsonConvert.SerializeObject(response);

                context.HttpContext.Response.StatusCode = 400;
                context.HttpContext.Response.ContentType = "application/json";

                await context.HttpContext.Response.WriteAsync(jsonObject);
            }
            else
            {
                await next();
            }
        }
    }
}

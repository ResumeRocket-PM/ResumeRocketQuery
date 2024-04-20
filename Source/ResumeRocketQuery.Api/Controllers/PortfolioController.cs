using System.Net;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    /// This is a controller that handles the creation and modification of accounts.
    /// </summary>
    [Authorize]
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;

        public PortfolioController(IServiceResponseBuilder serviceResponseBuilder)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
        }

        /// <summary>
        /// This retrieves the User details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("details")]
        public async Task<ServiceResponse<PortfolioResponseBody>> Get()
        {

            //User; //<-- This Object handles user state.
            var response = new PortfolioResponseBody
            {

            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
        }


        [HttpPost]
        [Route("details")]
        public async Task<ServiceResponse<object>> Post([FromBody] PortfolioRequestBody requestBody)
        {
            //user //<-- This Object handles user state.
            return _serviceResponseBuilder.BuildServiceResponse(new object(), HttpStatusCode.Created);
        }
    }


    public class PortfolioResponseBody
    {
        public string Content { get; set; }
    }

    public class PortfolioRequestBody
    {
        public string Content { get; set; }
    }
}
